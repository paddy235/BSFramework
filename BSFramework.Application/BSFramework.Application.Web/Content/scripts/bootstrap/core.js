$(document).ready(function () {
    $('input, textarea').placeholder();
});

    var Markdown = function () {
        // Class Properties
        this.$ns = 'bootstrap-markdown'
    }

    Markdown.prototype = {
       Grid: function (msg) {
            alert(msg);
        }
    }

