/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using net.r_eg.EvMSBuild;
using net.r_eg.vsSBE.Exceptions;

namespace net.r_eg.vsSBE.UI.WForms.Wizards.Version
{
    internal class Manager
    {
        /// <summary>
        /// Used environment.
        /// </summary>
        protected IEnvironment env;

        /// <summary>
        /// Used steps.
        /// </summary>
        protected Dictionary<StepsType, IStep> steps = new Dictionary<StepsType, IStep>();

        /// <summary>
        /// Step of generation.
        /// </summary>
        public StepGen StepGen
        {
            get { return (StepGen)getStep(StepsType.Gen); }
        }

        /// <summary>
        /// Step of configuring class or struct.
        /// </summary>
        public StepStruct StepStruct
        {
            get { return (StepStruct)getStep(StepsType.Struct); }
        }

        /// <summary>
        /// Step of direct replacing.
        /// </summary>
        public StepRepl StepRepl
        {
            get { return (StepRepl)getStep(StepsType.DirectRepl); }
        }

        /// <summary>
        /// Step of configuring general data.
        /// </summary>
        public StepCfgData StepCfgData
        {
            get { return (StepCfgData)getStep(StepsType.CfgData); }
        }

        /// <summary>
        /// Step of configuring fields.
        /// </summary>
        public StepFields StepFields
        {
            get { return (StepFields)getStep(StepsType.Fields); }
        }

        /// <summary>
        /// Final step.
        /// </summary>
        public StepFinal StepFinal
        {
            get { return (StepFinal)getStep(StepsType.Final); }
        }

        /// <param name="env">Current environment.</param>
        public Manager(IEnvironment env)
        {
            this.env = env;
        }

        /// <param name="type">The type of step.</param>
        /// <returns></returns>
        protected IStep getStep(StepsType type)
        {
            if(steps.ContainsKey(type)) {
                return steps[type];
            }

            switch(type)
            {
                case StepsType.Gen: {
                    steps[type] = new StepGen();
                    return steps[type];
                }
                case StepsType.Struct: {
                    steps[type] = new StepStruct(MSBuild.MakeEvaluator(env)) {
                        fnumber = StepStruct.NumberType.NativeStruct
                    };
                    return steps[type];
                }
                case StepsType.DirectRepl: {
                    steps[type] = new StepRepl();
                    return steps[type];
                }
                case StepsType.CfgData: {
                    steps[type] = new StepCfgData();
                    return steps[type];
                }
                case StepsType.Fields: {
                    steps[type] = new StepFields();
                    return steps[type];
                }
                case StepsType.Final:
                {
                    steps[type] = new StepFinal(this);
                    return steps[type];
                }
            }
            throw new NotSupportedException($"getStep: the type - `{type}` is not found.");
        }
    }
}
