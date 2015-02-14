/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace net.r_eg.vsSBE.UI.WForms
{
    public partial class AboutFrm: Form
    {
        protected class DeepSpace
        {
            protected struct Coord
            {
                public int x;
                public int y;
                public int w;
                public int h;
                public int layer;
            }

            public bool speedup;
            public int trail = 50;

            protected int width;
            protected int height;
            protected Graphics graphics;

            private Random random = new Random();
            private volatile bool abort = false;

            public void start(int maxStars)
            {
                abort = false;
                graphics.Clear(Color.Black);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

                Coord[] coord = new Coord[maxStars];

                for(int i = 0; i < maxStars; ++i) {
                    coord[i] = generate(rnd(0, height));
                }

                (new System.Threading.Tasks.Task(() => {
                    while(true)
                    {
                        for(int i = 1; i < 6; ++i) {
                            drawLayer(coord, i, i);
                        }
                        drawLayer(coord, 6, 8);

                        System.Threading.Thread.Sleep(40);
                        if(abort) {
                            return;
                        }
                        graphics.Clear(Color.Black);
                    }
                })).Start();
            }

            public void stop()
            {
                abort = true;
            }

            public DeepSpace(Graphics graphics, int width, int height)
            {
                this.graphics   = graphics;
                this.width      = width;
                this.height     = height;
            }

            protected Coord generate(int ypos, int hmax = -1)
            {
                int v = (hmax > 0)? hmax : rnd(1, 6);

                return new Coord() {
                    x       = rnd(0, width),
                    y       = ypos,
                    w       = rnd(1, 2),
                    h       = v,
                    layer   = v,
                };
            }

            protected int rnd(int min, int max)
            {
                return random.Next(min, max);
            }
            
            protected void drawLayer(Coord[] coord, int level, int speed)
            {
                for(int i = 0; i < coord.Length; ++i)
                {
                    if(coord[i].layer != level) {
                        continue;
                    }
                    graphics.FillRectangle((Brush)Brushes.White, coord[i].x, coord[i].y, coord[i].w, coord[i].h);

                    if(speedup && coord[i].layer < 3)
                    {
                        if(coord[i].h < trail) {
                            coord[i].h += speed;
                        }
                        coord[i].y += speed * 12;
                    }
                    else if(!speedup && coord[i].layer < 3 && coord[i].h > speed) {
                        coord[i].h -= speed;
                        coord[i].y += speed * 4;
                    }

                    coord[i].y += speed;

                    if(coord[i].y > height) {
                        coord[i] = generate(rnd(height * -1, 0));
                    }
                }
            }
        }
        protected DeepSpace space;

        public AboutFrm()
        {
            InitializeComponent();
            space = new DeepSpace(pictureBoxSpace.CreateGraphics(), pictureBoxSpace.Width, pictureBoxSpace.Height);

            labelCopyright.Text = String.Format("Copyright (c) 2013-{0}  Denis Kuzmin (reg) < entry.reg@gmail.com >", DateTime.Now.Year);

#if !DEBUG
            labelVersionVal.Text = String.Format("v{0} [ {1} ]", Version.numberWithRevString, Version.branchSha1);
            if(Version.branchName != "Releases") {
                labelVersionVal.Text += String.Format(" /\"{0}\":{1}", Version.branchName, Version.branchRevCount);
            }
#else
            labelVersionVal.Text = String.Format("v{0} Debug [ {1} ] /\"{2}\":{3}",
                                                    Version.numberWithRevString,
                                                    Version.branchSha1,
                                                    Version.branchName,
                                                    Version.branchRevCount);
#endif
        }

        private void AboutFrm_Load(object sender, EventArgs e)
        {
            space.start(350);
        }

        private void AboutFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            space.stop();
        }

        private void pictureBoxSpace_MouseDown(object sender, MouseEventArgs e)
        {
            space.speedup = true;
        }

        private void pictureBoxSpace_MouseUp(object sender, MouseEventArgs e)
        {
            space.speedup = false;
        }

        private void linkEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.openUrl("mailto:entry.reg@gmail.com");
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void linkLicense_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/raw/master/LICENSE");
        }

        private void linkPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.openUrl("http://r-eg.net");
        }

        private void linkLabelDonationHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/wiki/Donation");
        }

        private void pictureBoxDonation_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=P2HRG52AJSA9N&lc=US&item_name=vsSolutionBuildEvent%20%28vsSBE%29%20projects&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHosted");
        }
    }
}
