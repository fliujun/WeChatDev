var $dlgArticle, $dlgText, $dlgMethod;

$("#btnTextAdd").on('click', function (event) {
    event.preventDefault();
    showFormText();
});

function updateBtnWxText(id) {
    $("#formText input[name='id']").val(id);
    $.ajax({
        type: "POST",
        url: "MyHandler.ashx",
        data: { method: "getWxText", id: id },
        success: function (result) {
            var result = $.parseJSON(result);
            if (result.success) {
                $("#formText textarea[name='text']").val(result.data.Content);
            } else {
                show(result.error, false);
            }
        }
    });
    showFormText();
    $("#formText input[name='type']").val("update");
}

function showFormText() {
    $("#formText")[0].reset();
    if (!$dlgText) {
        $dlgText = $("#formText").dialog({
            autoOpen: false,
            title: "编辑文本",
            modal: true,
            buttons: [{
                text: "确定",
                click: function () {
                    var $this = $(this);
                    $("#formText").ajaxSubmit({
                        beforeSubmit: function (formData, jqForm, options) {
                            return true;
                        },
                        success: function (result) {
                            var result = $.parseJSON(result);
                            if (result.success) {
                                show("编辑成功！");
                                showWxTextList(result.data);
                                $("#formText")[0].reset();
                                $("#formText input[name='type']").val("add");
                                $this.dialog("close");
                            } else {
                                show(result.error, false);
                            }
                        }
                    });
                }
            }, {
                text: "取消",
                click: function () {
                    $(this).dialog("close");
                }
            }]
        });
    }
    $dlgText.dialog("open");
}

//显示微信文本列表
function showWxTextList(list) {
    if (list && list.length > 0) {
        var str1 = "", str2 = "", str3 = "";
        for (var i = 0; i < (list.length) ; i = i + 3) {
            var item1 = list[i], item2 = list[i + 1]; item3 = list[i + 2];
            if (item1) {
                str1 = str1 + "<div class='wxText'>" +
                "<div class='time'>" + item1.UpdateTime + "</div><hr/>" +
                "<div class='txt'>" + item1.Content + "</div><hr/>" +
            "<div class='btn-group'><button class='btn btn-default btn-sm' onclick='updateBtnWxText(" + item1.ID + ");'>修改</button><button class='btn btn-default btn-sm' onclick='deleteWxText(" + item1.ID + ");'>删除</button><button class='btn btn-default btn-sm' onclick='viexWxText(" + item1.ID + ");'>绑定</button></div>" +
            "</div>";
            }
            if (item2) {
                str2 = str2 + "<div class='wxText'>" +
                 "<div class='time'>" + item2.UpdateTime + "</div><hr/>" +
                "<div class='txt'>" + item2.Content + "</div><hr/>" +
            "<div class='btn-group'><button class='btn btn-default btn-sm' onclick='updateBtnWxText(" + item2.ID + ");'>修改</button><button class='btn btn-default btn-sm' onclick='deleteWxText(" + item2.ID + ");'>删除</button><button class='btn btn-default btn-sm' onclick='viexWxText(" + item2.ID + ");'>绑定</button></div>" +
            "</div>";
            }
            if (item3) {
                str3 = str3 + "<div class='wxText'>" +
                 "<div class='time'>" + item3.UpdateTime + "</div><hr/>" +
                "<div class='txt'>" + item3.Content + "</div><hr/>" +
            "<div class='btn-group'><button class='btn btn-default btn-sm' onclick='updateBtnWxText(" + item3.ID + ");'>修改</button><button class='btn btn-default btn-sm' onclick='deleteWxText(" + item3.ID + ");'>删除</button><button class='btn btn-default btn-sm' onclick='viexWxText(" + item3.ID + ");'>绑定</button></div>" +
            "</div>";
            }
        }
        $("#showWxText1").html(str1);
        $("#showWxText2").html(str2);
        $("#showWxText3").html(str3);
    } else {
        getWxTextList();
    }
}

//获取微信文本列表
function getWxTextList() {
    $.ajax({
        type: "POST",
        url: "MyHandler.ashx",
        data: { method: "getWxTextList" },
        success: function (result) {
            var result = $.parseJSON(result);
            if (result.success) {
                showWxTextList(result.data);
            } else {
                show(result.error, false);
            }
        }
    });
}

//删除微信文本
function deleteWxText(id) {
    if (confirm("是否删除？")) {
        $.ajax({
            type: "POST",
            url: "MyHandler.ashx",
            data: { method: "deleteWxText", id: id },
            success: function (result) {
                var result = $.parseJSON(result);
                if (result.success) {
                    show("删除成功！");
                    showWxTextList(result.data);
                    $("#wxText").val("");
                } else {
                    show(result.error, false);
                }
            }
        });
    }
}