/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.IO;
using System.Text;
using net.r_eg.vsSBE.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;

namespace net.r_eg.vsSBE.Configuration
{
    /// <summary>
    /// Basic logic for servicing objects to implement configurations.
    /// </summary>
    /// <remarks>
    /// TOut should be equivalent to TIn, for example: 
    /// extending TIn or have a common interface, or simple equal to TIn.
    /// </remarks>
    /// <typeparam name="TIn">Abstract or specific type for serialization.</typeparam>
    /// <typeparam name="TOut">
    /// A more specific type of implementation used for deserialization 
    /// in an attempt to work without custom converters and TypeNameHandling.
    /// </typeparam>
    public abstract class PackerAbstract<TIn, TOut>: IConfig<TIn>
        where TOut: TIn, new()
    {
        /// <inheritdoc cref="loadFrom(string, Action{TIn}, bool, Action{JsonException})"/>
        protected abstract bool loadFrom(string link);

        public event EventHandler<DataArgs<TIn>> Updated = delegate (object sender, DataArgs<TIn> e) { };

        public virtual string EntityName { get; } = Settings.APP_CFG;

        public virtual string EntityExt { get; }

        public TIn Data { get; protected set; }

        public string Link { get; protected set; }

        public bool InRAM { get; protected set; }

        public void load(TIn data) => loadAndTriggerUpdated(data);

        public virtual bool loadPath(string path, string prefix = null)
        {
            Link = getLink(path, EntityName + EntityExt, prefix);
            return loadFrom(Link);
        }

        public virtual bool load(string link)
        {
            Link = link + EntityExt;
            return loadFrom(Link);
        }

        public void unload()
        {
            Link = null;
            loadAndTriggerUpdated(default);
        }

        public virtual void save()
        {
            if(Link == null)
            {
                Log.Trace($"{GetType().Name}: Ignore saving. Link is null.");
                return;
            }

            try
            {
                using(TextWriter stream = new StreamWriter(Link, false, Encoding.UTF8))
                {
                    serialize(stream, Data);
                }

                InRAM = false;

                Log.Trace($"{GetType().Name} has been updated {Link}");
                triggerUpdated(Data);
            }
            catch(Exception ex)
            {
                Log.Error($"Unable to apply configuration: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
        }

        protected void triggerUpdated(TIn data)
            => Updated(this, new DataArgs<TIn>() { Data = data });

        protected void loadAndTriggerUpdated(TIn data)
        {
            Data = data;
            triggerUpdated(data);
        }

        /// <param name="path">Base path.</param>
        /// <param name="name">File name.</param>
        /// <param name="prefix">File prefix if used.</param>
        /// <returns>Full path to configuration file.</returns>
        protected string getLink(string path, string name, string prefix = null)
        {
            string link = FormatLink(path, name, prefix);
            if(!File.Exists(link))
            {
                Log.Debug($"Configuration file '{link}' is not found. '{name}':'{prefix}'");
                link = FormatLink(path, name);
            }
            return link;
        }

        /// <summary>
        /// Load and deserialize configuration.
        /// </summary>
        /// <param name="link">Link to configuration file.</param>
        /// <param name="onSuccess">Extra if successful.</param>
        /// <param name="importance">Affects verbosity.</param>
        /// <param name="onJsonException">Extra if caught <see cref="JsonException"/>.</param>
        /// <returns>true value if loaded from existing file, otherwise loaded as new.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        protected virtual bool loadFrom
            (
                string link,
                Action<TIn> onSuccess,
                bool importance = false,
                Action<JsonException> onJsonException = null
            )
        {
            if(link == null) throw new ArgumentNullException(nameof(link));

            InRAM = false;
            try
            {
                using(StreamReader stream = new(link, Encoding.UTF8, true))
                {
                    Data = deserialize(stream)
                        ?? throw new UnspecSBEException($"Failed {nameof(deserialize)}");
                }
                onSuccess?.Invoke(Data);
            }
            catch(FileNotFoundException)
            {
                NewDataInRAM(importance);
            }
            catch(JsonException ex)
            {
                onJsonException?.Invoke(ex);
                NewDataInRAM();
            }
            catch(Exception ex)
            {
                //TODO: actions in UI, e.g.: restore, new..
                NewDataInRAM(importance, link, ex);

                if(importance) Log.Debug(ex.StackTrace);
            }

            triggerUpdated(Data);
            return !InRAM;
        }

        protected string serialize(TIn data)
        {
            JsonSerializerSettings settings = new()
            {
                NullValueHandling       = NullValueHandling.Ignore,
                StringEscapeHandling    = StringEscapeHandling.EscapeNonAscii,
                Formatting              = Formatting.Indented,
                MissingMemberHandling   = MissingMemberHandling.Ignore,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            };

            settings.Converters.Add(new StringEnumConverter { 
                AllowIntegerValues = false
            });

            return JsonConvert.SerializeObject(data, Formatting.Indented, settings);
        }

        protected TOut deserialize(StreamReader stream)
        {
            using JsonTextReader reader = new(stream);
            JsonSerializer js = new()
            {
                SerializationBinder = new JsonSerializationBinder()
            };
            return js.Deserialize<TOut>(reader);
        }

        protected void serialize(TextWriter stream, TIn data)
            => stream.Write(serialize(data));

        private static string FormatLink(string path, string name, string prefix = null)
            => Path.Combine(path, $"{prefix}{name}");

        private void NewDataInRAM
            (
                bool? importance = null,
                string link = null,
                Exception failed = null
            )
        {
            Data    = new TOut();
            InRAM   = true;

            if(importance == null) return;

            if(failed == null)
            {
                Log.Msg
                (
                    importance == true ? LogLevel.Info : LogLevel.Trace,
                    $"Configuration {GetType().Name}: Initialized as new {link}"
                );
            }
            else
            {
                Log.Msg
                (
                    importance == true ? LogLevel.Error : LogLevel.Debug,
                    $"{GetType().Name}: Failed {link} due to {failed.Message}"
                );
            }
        }
    }
}
