/*
 * Copyright (c) 2013-2016,2019  Denis Kuzmin < entry.reg@gmail.com > GitHub/3F
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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace net.r_eg.vsSBE.UI.WForms
{
    public partial class AboutFrm: Form
    {
        protected class DeepSpace
        {
            public bool speedup;

            protected struct Coord
            {
                public int x, y;
                public int w, h;
                public int layer;
                public Object prev;
            }

            protected int width, height;
            protected Graphics graphics;
            protected List<Brush> shades = new List<Brush>();

            private Random random = new Random();
            private volatile bool abort = false;

            public void start(int stars)
            {
                abort = false;
                Coord[] coord = new Coord[stars];

                for(int i = 0; i < stars; ++i) {
                    coord[i] = generate(rnd(0, height));
                }

                (new System.Threading.Tasks.Task(() => {
                    while(true)
                    {
                        for(int i = 1; i < 6; ++i) {
                            drawLayer(coord, i, i);
                        }
                        drawLayer(coord, 6, 8);

                        if(abort) {
                            abort = false;
                            return;
                        }
                        System.Threading.Thread.Sleep(40);
                    }
                }))
                .Start();
            }

            public void stop()
            {
                abort = true;
                while(abort) {
                    System.Threading.Thread.Sleep(10);
                }
            }

            public DeepSpace(Graphics graphics, int width, int height)
            {
                this.graphics   = graphics;
                this.width      = width;
                this.height     = height;

                for(int c = 0xFF; c > 0; c -= 0x0F) {
                    shades.Add(new SolidBrush(Color.FromArgb(c, c, c)));
                }

                graphics.SmoothingMode      = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                graphics.InterpolationMode  = System.Drawing.Drawing2D.InterpolationMode.Low;
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
                    clear(coord[i].prev);
                    drawStar(coord[i]);

                    coord[i].prev = new Coord() {
                        x = coord[i].x,
                        y = coord[i].y,
                        w = coord[i].w,
                        h = coord[i].h,
                        layer = coord[i].layer
                    };

                    if(speedup && coord[i].layer < 3)
                    {
                        if(coord[i].h < 50) {
                            coord[i].h += speed;
                        }
                        coord[i].y += speed * 12;
                    }
                    else if(!speedup && coord[i].layer < 3 && coord[i].h > speed) {
                        coord[i].h -= speed;
                        coord[i].y += speed * 6;
                    }

                    coord[i].y += speed;

                    if(coord[i].y > height) {
                        clear(coord[i].prev);
                        coord[i] = generate(rnd(height * -1, 0));
                    }
                }
            }

            protected void drawStar(Coord point)
            {
                if(point.h == 1) {
                    graphics.FillRectangle(shades[rnd(1, 5)], point.x, point.y, point.w, point.h);
                    return;
                }
                int shadeOn = shades.Count - 1;

                if(point.h <= 6) { // short tail variant
                    int shadeN = rnd(0, (int)(shadeOn * 0.56f));
                    graphics.FillRectangle(shades[shadeN], point.x, point.y + 2, point.w, point.h - 2);
                    graphics.FillRectangle(shades[Math.Min(shadeOn, shadeN + 4)], point.x, point.y, point.w, 1);
                    graphics.FillRectangle(shades[Math.Min(shadeOn, shadeN + 2)], point.x, point.y + 1, point.w, 1);
                    return;
                }

                int body = point.h - 2;
                graphics.FillRectangle(Brushes.White, point.x, point.y + body, point.w, 2);
                for(int i = 1; i < body; i += 2) {
                    graphics.FillRectangle(shades[Math.Min((int)(shadeOn * 0.75f), body - i)], point.x, point.y + i, point.w, 2);
                }
            }

            protected void clear(object coord)
            {
                if(coord == null) {
                    return;
                }
                Coord prev = (Coord)coord;
                graphics.FillRectangle(Brushes.Black, prev.x, prev.y, prev.w, prev.h);
            }
        }
        protected DeepSpace space;


        public AboutFrm()
        {
            InitializeComponent();
            Icon = Resource.Package_32;

            space = new DeepSpace(pictureBoxSpace.CreateGraphics(), pictureBoxSpace.Width, pictureBoxSpace.Height);

            labelCopyright.Text = String.Format("Copyright (c) 2013-{0}  Denis Kuzmin <entry.reg@gmail.com> GitHub/3F", Math.Max(2019, DateTime.Now.Year));
            string vAPIString   = (new API.Version()).Bridge.Number.ToString(2);

#if !DEBUG
            labelVersionVal.Text = String.Format("v{0} [ {1} ] API: v{2}", Version.numberWithRevString, Version.branchSha1, vAPIString);
            if(Version.branchName.ToLower() != "releases") {
                labelVersionVal.Text += String.Format(" /\"{0}\":{1}", Version.branchName, Version.branchRevCount);
            }
#else
            labelVersionVal.Text = String.Format("v{0} Debug [ {1} ] API: v{2} /\"{3}\":{4}",
                                                    Version.numberWithRevString,
                                                    Version.branchSha1,
                                                    vAPIString,
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
            Util.openUrl("https://github.com/3F/vsSolutionBuildEvent/blob/master/LICENSE");
        }

        private void linkPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.openUrl("https://github.com/3F");
        }

        private void linkLabelDonationHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.openUrl("https://3F.github.com/Donation/");
        }

        private void pictureBoxDonation_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=entry%2ereg%40gmail%2ecom&lc=US&item_name=Open%20Source%20%5b%20GitHub%2F3F&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_SM%2egif%3aNonHosted");
        }

        private void pictureBoxDonation_MouseHover(object sender, EventArgs e)
        {
            pictureBoxDonation.Padding = new Padding(1);
            pictureBoxDonation.Refresh();
        }

        private void pictureBoxDonation_MouseLeave(object sender, EventArgs e)
        {
            pictureBoxDonation.Padding = new Padding(0);
            pictureBoxDonation.Refresh();
        }
    }
}
