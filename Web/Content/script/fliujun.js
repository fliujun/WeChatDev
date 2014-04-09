//来自于liujun的插件库，官网：www.fliujun.com

//No.1智能提示框
(function ($) {
    var mydiv, timeout;
    var defaults = {
        str: '请输入提示文字',
        success: true
    };
    
    if (!mydiv) {
        $("body").append("<div id='fliujunShow' style=''></div>");
        mydiv = $("#fliujunShow");
        var mycss = { width: "350px", height: "100px", "line-height": "100px", "background": "#45cb2f", "position": "fixed", "text-align": "center", "font-size": "16px", "font-weight": "bold", "color": "#fff", "display": "none", "overflow": "hidden" };
        mydiv.css(mycss);
        $(mydiv).hover(function () {
            mydiv.css({ "overflow": "auto", "box-shadow": "1px 1px 10px @bgColorDeep" });
        }, function () {
            mydiv.css({ "overflow": "hidden", "box-shadow": "0px 0px 0px @bgColorDeep" });
        });
        mydiv.hover(function () {
            clearTimeout(timeout);
            mydiv.fadeIn();
        }, function () {
            timeout = setTimeout(function () {
                mydiv.fadeOut(500);
            }, 1500);
        });
    }

    $.fn.show = function (options) {
        window.console.log("开始");
        var opts = $.extend(defaults, options);
        clearTimeout(timeout);
        mydiv.text(opts.str);
        if (opts.success) {
            mydiv.css("background", "#4bd3ac");
        } else {
            mydiv.css("background", "rgb(255,0,0)");
        }
        var top = ($(window).height() - mydiv.height()) / 2;
        var left = ($(window).width() - mydiv.width()) / 2;
        var scrollTop = $(document).scrollTop();
        var scrollLeft = $(document).scrollLeft();
        mydiv.css({ position: 'absolute', 'top': top + scrollTop, left: left + scrollLeft }).fadeIn();
        timeout = setTimeout(function () {
            mydiv.fadeOut(300);
        }, 1500);
        window.console.log("结束");
    };

    $.fn.show.defaults = {
        height: "100px",
        width: "350px"
    };
})(jQuery);