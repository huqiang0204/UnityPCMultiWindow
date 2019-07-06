using huqiang.Data;
using huqiang.UI;
using huqiang.UIComposite;
using huqiang.UIEvent;
using System.Collections.Generic;
using UnityEngine;
using Windows;

public class UITest : TestHelper
{
    WinForm win;
    public Camera camera;
    public Canvas canvas;
    class View
    {
        public EventCallBack but;
    }
    View view;
    public override void LoadTestPage()
    {
        Application.targetFrameRate = 60;
#if UNITY_IPHONE || UNITY_ANDROID
        //Scale.DpiScale = true;
#endif
        //UIPage.LoadPage<DrawPage>();
        //UIPage.LoadPage<LayoutTestPage>();
        //UIPage.LoadPage<TestPage>();
        win = new WinForm("tool");
        win.SetCanvas(canvas.transform as RectTransform);
        win.BindingCamera(camera,canvas);
        var page = win.AddNode("page");
        var model = ModelManagerUI.CloneModel("baseUI","testpage");
        view = new View();
        ModelManagerUI.LoadToGame(model, view);
        model.SetParent(page);
        view.but.Click = (o, e) => { Debug.Log("click is ok"); };
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        win.UpdateRender();
    }
}