//绑定ajax请求显示加载信息
$(document).ajaxStart(function () {
    $("#loading").show();
});

//绑定ajax请求完毕隐藏加载信息
$(document).ajaxComplete(function () {
    $("#loading").hide();
});

//绑定ajax请求错误时
$(document).ajaxError(function (event, request, settings) {
    show("请求出错：" + settings.url);
});


//显示提示信息
var timeout;
function show(str, success) {
    clearTimeout(timeout);
    $("#alert").text(str);
    if (success || success == undefined) {
        $("#alert").css("background", "#4bd3ac");
    } else {
        $("#alert").css("background", "rgb(255,0,0)");
    }
    var divName = "#alert";
    var top = ($(window).height() - $(divName).height()) / 2;
    var left = ($(window).width() - $(divName).width()) / 2;
    var scrollTop = $(document).scrollTop();
    var scrollLeft = $(document).scrollLeft();
    $(divName).css({ position: 'absolute', 'top': top + scrollTop, left: left + scrollLeft }).fadeIn();

    timeout = setTimeout(function () {
        $("#alert").fadeOut(300);
    }, 1500);
}
$("#alert").hover(function () {
    clearTimeout(timeout);
    $("#alert").fadeIn();
}, function () {
    timeout = setTimeout(function () {
        $("#alert").fadeOut(500);
    }, 1500);
});

//获取字符串长度，汉字占两个字节
function getLength(str) {
    var totalCount = 0;
    for (var i = 0; i < str.length; i++) {
        var c = str.charCodeAt(i);
        if ((c >= 0x0001 && c <= 0x007e) || (0xff60 <= c && c <= 0xff9f)) {
            totalCount++;
        }
        else {
            totalCount += 2;
        }
    }
    return totalCount;
}

/********************* 菜单树操作 ************************/
//创建节点
function Tcreate() {
    var ref = $('#mTree').jstree(true),
        sel = ref.get_selected();
    if (!sel.length) { return false; }
    sel = sel[0];
    var type = "main";
    if (sel.indexOf('m') > -1) {
        type = "submenu";
    }
    sel = ref.create_node(sel, { "type": type });
    //if (sel) {
    //    ref.edit(sel);
    //}
};
$('#mTree').on("create_node.jstree", function (e, data) {
    var ref = $('#mTree').jstree(true),
    sel = ref.get_selected();
    if (!sel.length) { return false; }
    sel = sel[0];

    var pid = -1;
    if (sel.indexOf('m') > -1) {
        pid = sel.replace('m', '');
    }
    var text = $.trim(data.node.text);

    if (pid < 0 && getLength(text) > 16) {
        show("一级菜单名称长度不能超过16字节", false);
        ref.edit(sel);
        return;
    } else if (pid > 0 && getLength(text) > 40) {
        show("二级菜单名称长度不能超过40字节", false);
        ref.edit(sel);
        return;
    }

    $.ajax({
        type: "POST",
        url: "../MyHandler.ashx",
        data: { method: "addMenu", pid: pid, text: text, type: data.node.type, ordernum: data.position },
        success: function (result) {
            var result = $.parseJSON(result);
            if (result.success) {
                show("添加成功");
            } else {
                show("添加失败：" + result.error, false);
            }
            loadTreeData();
        },
        error: function (e) {
            show(e, false);
        }
    });
});

//重命名节点
function Trename() {
    var ref = $('#mTree').jstree(true),
        sel = ref.get_selected();
    if (ref.get_node(sel).id.indexOf("r") < 0) {
        if (!sel.length) { return false; }
        sel = sel[0];
        ref.edit(sel);
    } else {
        show("不允许操作此节点", false);
    }
};
$('#mTree').on("rename_node.jstree", function (e, data) {
    var ref = $('#mTree').jstree(true),
    sel = ref.get_selected();
    if (!sel.length) { return false; }
    sel = sel[0];

    var text = $.trim(data.text);
    if (data.old != text) {
        if (data.node.type == "main" && getLength(text) > 16) {
            show("一级菜单名称长度不能超过16字节", false);
            ref.edit(sel);
            return;
        } else if (data.node.type == "submenu" && getLength(text) > 40) {
            show("二级菜单名称长度不能超过40字节", false);
            ref.edit(sel);
            return;
        }

        $.ajax({
            type: "POST",
            url: "MyHandler.ashx",
            data: { method: "updateMenuName", "id": data.node.data.id, "text": text },
            success: function (result) {
                var result = $.parseJSON(result);
                if (result.success) {
                    show("修改成功");
                } else {
                    ref.edit(sel, data.old);
                    show("修改失败：" + result.error, false);
                }
            }
        });
    }
});

//删除节点
function Tdelete() {
    var ref = $('#mTree').jstree(true),
        sel = ref.get_selected();
    if (sel[0]) {
        if (sel[0].indexOf("r") > -1) {
            show("不允许删除此节点", false);
            return;
        }
        if (confirm("是否删除？")) {
            if (!sel.length) { return false; }
            ref.delete_node(sel);
        }
    }
};
$('#mTree').on("delete_node.jstree", function (e, data) {
    $.ajax({
        type: "POST",
        url: "MyHandler.ashx",
        data: { method: "deleteMenu", "id": data.node.data.id },
        success: function (result) {
            var result = $.parseJSON(result);
            if (result.success) {
                show("删除成功");
            } else {
                show("删除失败：" + result.error, false);
                loadTreeData();
            }
        }
    });
});

//称动节点
$('#mTree').on("move_node.jstree.jstree", function (e, data) {
    var index = data.position;
    var id = data.node.data.id;
    var newtype = data.parent;
    var pid = -1;
    if (data.parent.indexOf('m') > -1) {
        pid = newtype.replace('m', '');
    }

    $.ajax({
        type: "POST",
        url: "MyHandler.ashx",
        data: { method: "updateMenuOrder", id: id, pid: pid, ordernum: index },
        success: function (result) {
            var result = $.parseJSON(result);
            if (result.success) {
                show("修改成功");
            } else {
                show("修改失败：" + result.error, false);
            }
            loadTreeData();
        },
        error: function (e) {
            loadTreeData();
            show(e, false);
        }
    });
});

//动态加载菜单树数据
var treedata = ["正在加载..."];
function loadTreeData(isInit) {
    $.ajax({
        type: "POST",
        url: "MyHandler.ashx",
        data: { method: "loadTreeData" },
        success: function (result) {
            var result = $.parseJSON(result);
            if (result.success) {
                //初始化分类
                treedata = $.parseJSON(result.data);
                $("#mTree").jstree(true).refresh();
                if (isInit) {
                    showWxTextList();
                }
            } else {
                show(result.error, false);
            }

        }
    });
}

//树搜索
var to = false;
$('#tQuery').keyup(function () {
    if (to) { clearTimeout(to); }
    to = setTimeout(function () {
        var v = $('#tQuery').val();
        $('#mTree').jstree(true).search(v);
    }, 250);
});


//同步微信菜单
function SyncMenu() {
    if (confirm("是否立即同步至微信？")) {
        $.ajax({
            type: "POST",
            url: "MyHandler.ashx",
            data: { method: "syncWxMenu" },
            success: function (result) {
                var result = $.parseJSON(result);
                if (result.success) {
                    //初始化分类
                    show("同步至微信成功！");
                } else {
                    show(result.error, false);
                }
            }
        });
    }
}

/********************* Jquery入口 ************************/
$(function () {
    //初始化菜单树
    $('#mTree').jstree({
        'core': {
            "themes": { "stripes": false, "responsive": true },
            'data': function (obj, callback) {
                callback.call(this, treedata);
            },
            "check_callback": true,
            'strings': {
                'Loading...': '正在加载...'
            },
            "multiple": false
        },
        "types": {
            "default": {
                "icon": "glyphicon glyphicon-stop"
            },
            "#": {
                "max_children": 3,
                "max_depth": 3,
                "icon": "glyphicon glyphicon-th-large"
            },
            "main": {
                "max_children": 5,
                "icon": "glyphicon glyphicon-stop"
            },
            "submenu": {
                "icon": "glyphicon glyphicon-leaf"
            }
        },
        "plugins": [
            //"contextmenu",
            "dnd", "search", "state", "types"],
        search: {
            show_only_matches: true
        }
    });
    loadTreeData(true);
});