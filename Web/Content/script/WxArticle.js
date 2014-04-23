$('#btnArticleAdd').on('click', function (event) {
    event.preventDefault();
    if (!$dlgArticle) {
        $dlgArticle = $('#formArticle').dialog({
            autoOpen: false,
            title: "编辑图文",
            modal: true,
            width: "60%",
            open: function () {
                editor = KindEditor.create('#inputContent', {
                    cssPath: 'Content/kindeditor/plugins/code/prettify.css',
                    uploadJson: 'upload_json.ashx',
                    fileManagerJson: 'file_manager_json.ashx',
                    allowFileManager: true,
                    afterCreate: function () {
                    },
                    //当失去焦点时执行 this.sync();
                    afterBlur: function () { this.sync(); }
                });
                prettyPrint();
            },
            buttons: [{
                text: "确定",
                click: function () {
                    $(this).dialog("close");
                }
            }, {
                text: "取消",
                click: function () {
                    $(this).dialog("close");
                }
            }]
        });
    }
    $dlgArticle.dialog("open");
});