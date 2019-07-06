using huqiang.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;
using huqiang.UIEvent;

namespace Windows
{
    public class WinForm:RenderForm
    {
        Form form;
        UserAction action;
        UserAction tmpAction;
        public WinForm(string name):base (name)
        {
            form = new Form();
            form.Name = name;
            form.ClientSize = new System.Drawing.Size(256,256);
            form.Show();
            form.FormClosing += CloseForm;
            form.SizeChanged += SizeChanged;
            form.MouseDown += MouseDown;
            form.MouseUp += MouseUp;
            form.MouseWheel += MouseWheel;
            form.MouseMove += MouseMove;
            form.KeyDown += KeyDown;
            form.KeyPress += KeyPress;
            form.KeyUp += KeyUp;
            inputs = new UserAction[1];
            inputs[0] = action = new UserAction(0);
            tmpAction = new UserAction(1);
            tmpAction.IsActive = true;
            Size = new Vector2(256,256);
        }
        public override void Dispose()
        {
            form.FormClosing -= CloseForm;
            form.SizeChanged -= SizeChanged;
            form.MouseDown -= MouseDown;
            form.MouseUp -= MouseUp;
            form.MouseWheel -= MouseWheel;
            form.MouseMove -= MouseMove;
            form.KeyDown -= KeyDown;
            form.KeyPress -= KeyPress;
            form.KeyUp -= KeyUp;
            form = null;
        }
        public override void LoadUserAction()
        {
            action.CopyAction(tmpAction,Size);
            tmpAction.IsLeftButtonDown = false;
            tmpAction.IsLeftButtonUp = false;
            tmpAction.IsMiddleButtonDown = false;
            tmpAction.IsMiddleButtonUp = false;
            tmpAction.IsRightButtonDown = false;
            tmpAction.IsRightButtonUp = false;
        }
        void CloseForm(object sender, FormClosingEventArgs e)
        {
            Dispose();
            if (Close != null)
                Close(this);
        }
        void SizeChanged(object sender, EventArgs e)
        {
            int w = form.ClientSize.Width;
            Size.x = w;
            int h = form.ClientSize.Height;
            Size.y = h;
            if(bitmap!=null)
            bitmap.Dispose();
            bitmap = null;
            if (maincan != null)
                (maincan.transform as RectTransform).sizeDelta = Size;
            if (maincam != null)
            {
                var tex = maincam.targetTexture;
                RenderTexture render = new RenderTexture(new RenderTextureDescriptor(w, h));
                maincam.targetTexture = render;
                GameObject.Destroy(tex);
            }
            if (ReSized != null)
                ReSized(this);
        }
        void MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    tmpAction.IsLeftButtonDown = true;
                    tmpAction.IsLeftButtonUp = false;
                    tmpAction.isPressed = true;
                    break;
                case MouseButtons.Middle:
                    tmpAction.IsMiddleButtonDown= true;
                    tmpAction.IsMiddleButtonUp = false;
                    tmpAction.IsMiddlePressed = true;
                    break;
                case MouseButtons.Right:
                    tmpAction.IsRightButtonDown = true;
                    tmpAction.IsRightButtonUp = false;
                    tmpAction.IsRightPressed = true;
                    break;
            }
            tmpAction.Position.x = e.X;
            tmpAction.Position.y =Size.y- e.Y;
        }
        void MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    tmpAction.IsLeftButtonUp = true;
                    tmpAction.IsLeftButtonDown = false;
                    tmpAction.isPressed = false;
                    break;
                case MouseButtons.Middle:
                    tmpAction.IsMiddleButtonUp = true;
                    tmpAction.IsMiddleButtonDown = false;
                    tmpAction.IsMiddlePressed = false;
                    break;
                case MouseButtons.Right:
                    tmpAction.IsRightButtonUp = true;
                    tmpAction.IsRightButtonDown = false;
                    tmpAction.IsRightPressed = false;
                    break;
            }
            tmpAction.Position.x = e.X;
            tmpAction.Position.y =Size.y - e.Y;
        }
        void MouseWheel(object sender, MouseEventArgs e)
        {
            tmpAction.MouseWheelDelta = e.Delta;
            tmpAction.Position.x = e.X;
            tmpAction.Position.y =Size.y- e.Y;
        }
        void MouseMove(object sender, MouseEventArgs e)
        {
            tmpAction.IsMoved = true;
            tmpAction.Position.x = e.X;
            tmpAction.Position.y =Size.y - e.Y;
        }
        void KeyDown(object sender, KeyEventArgs e)
        {

        }
        void KeyPress(object sender, KeyPressEventArgs e)
        {

        }
        void KeyUp(object sender, KeyEventArgs e)
        {

        }
        public void Draw(UnityEngine.Color32[] color)
        {
            int w = form.ClientSize.Width;
            int h = form.ClientSize.Height;
            int len = w * h;
            if (len == color.Length)
            {
                if (bitmap == null)
                    bitmap = new System.Drawing.Bitmap(w, h);
                else if(bitmap.Width!=w|bitmap.Height!=h)
                {
                    bitmap.Dispose();
                    bitmap = new System.Drawing.Bitmap(w,h);
                }
                int s = 0;
                for (int i = 0; i < h; i++)
                {
                    int y = h - i - 1;
                    for (int j = 0; j < w; j++)
                    {
                        UnityEngine.Color32 col = color[s];
                        var tmp = System.Drawing.Color.FromArgb(255, col.r, col.g, col.b);
                        bitmap.SetPixel(j, y, tmp);
                        s++;
                    }
                }
                var g = System.Drawing.Graphics.FromHwnd(form.Handle);
                g.DrawImage(bitmap, 0, 0, w, h);
                g.Dispose();
            }
        }
        System.Drawing.Bitmap bitmap;
        public Vector2 Size;
        public Action<WinForm> ReSized;
        public Action<WinForm> Close;
        Camera maincam;
        Canvas maincan;
        public void BindingCamera(Camera camera,Canvas canvas)
        {
            maincam = camera;
            maincan = canvas;
        }
        public void UpdateRender()
        {
            if (form != null)
            {
                if (maincam == null)
                    return;
                RenderTexture renderTexture = maincam.targetTexture;//拿到目标渲染纹理
                if (renderTexture == null)
                    return;
                RenderTexture.active = renderTexture;
                Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);//新建纹理存储渲染纹理

                tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);//把渲染纹理的像素给Texture2D,才能在项目里面使用
                tex.Apply();//记得应用一下，不然很蛋疼
                var colors = tex.GetPixels32();
                GameObject.Destroy(tex);
                Draw(colors);
            }
        }
    }
}
