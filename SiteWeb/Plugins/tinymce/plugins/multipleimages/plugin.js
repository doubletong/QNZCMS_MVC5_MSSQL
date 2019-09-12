tinymce.PluginManager.add('multipleimages', function (editor, url) {
    // Add a button that opens a window
    window.pumppipe = function (e) {
       // alert("pumppipe" + JSON.stringify(e)); 
        var html = "";
        e.forEach(function (element) {
            html += '<img src="' + element + '" alt="" />';
        });

        // var dialogArguments = top.tinymce.activeEditor.windowManager.getParams();  
        top.tinymce.activeEditor.insertContent(html);
    };

    editor.addButton('multipleimages', {
        text: '多图选择',
        icon: 'image',
        onclick: function () {
            // Open window
            openFinder();
        }
    });

    // Adds a menu item to the tools menu
    editor.addMenuItem('multipleimages', {
        text: '多图选择',
        context: 'tools',
        onclick: function () {
            // Open window with a specific url
            openFinder();
        }
    });


    function openFinder() {
        editor.windowManager.open({
            title: 'TinyMCE site',
            url: '/bbi-admin/File/FinderForMultipleImages',
            width: 900,
            height: 660

        });
    }


    return {
        getMetadata: function () {
            return {
                name: "Example plugin",
                url: "http://exampleplugindocsurl.com"
            };
        }
    };
});