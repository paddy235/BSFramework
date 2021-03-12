


var keyValue = $("#BaseId").val();


jQuery(function () {
    var $ = jQuery,    // just in case. Make sure it's not an other libaray.

        $wrap = $('#uploader'),

        // 图片容器
        $queue = $('<div class="filelist"></div>')
            .appendTo($wrap.find('.queueList')),

        // 状态栏，包括进度和控制按钮
        $statusBar = $wrap.find('.statusBar'),

        // 文件总体选择信息。
        $info = $statusBar.find('.info'),



        // 没选择文件之前的内容。
        $placeHolder = $wrap.find('.placeholder'),

        // 总体进度条
        $progress = $statusBar.find('.progress').hide(),

        // 添加的文件数量
        fileCount = 0,

        // 添加的文件总大小
        fileSize = 0,

        // 优化retina, 在retina下这个值是2
        ratio = window.devicePixelRatio || 1,

        // 缩略图大小
        thumbnailWidth = 110 * ratio,
        thumbnailHeight = 110 * ratio,

        // 可能有pedding, ready, uploading, confirm, done.
        state = 'pedding',

        // 所有文件的进度信息，key为file id
        percentages = {},

        supportTransition = (function () {
            var s = document.createElement('p').style,
                r = 'transition' in s ||
                      'WebkitTransition' in s ||
                      'MozTransition' in s ||
                      'msTransition' in s ||
                      'OTransition' in s;
            s = null;
            return r;
        })(),

        // WebUploader实例
        uploader;

    if (!WebUploader.Uploader.support()) {
        alert('Web Uploader 不支持您的浏览器！如果你使用的是IE浏览器，请尝试升级 flash 播放器');
        throw new Error('WebUploader does not support the browser you are using.');
    }

    // 实例化
    uploader = WebUploader.create({
        auto: true,
        pick: {
            id: '#filePicker',
            innerHTML: ('点击选择文件')
        },
        dnd: '#uploader .queueList',
        paste: document.body,

        accept: {
            title: '常用文档',
            extensions: 'mp4,doc,docx,txt,pdf,ppt,xls,xlsx,jpg,png',
            mimeTypes: '*/*'
        },

        // swf文件路径
        swf: '/../content/scripts/plugins/webuploader/Uploader.swf',

        disableGlobalDnd: true,
        chunked: false,
        server: '../../../Utility/PostFileActity?recId=' + keyValue + "&filePath=Safetyday&ptype=0&isDate=1",
        fileNumLimit: 5,
        fileSizeLimit: 500 * 1024 * 1024,
        fileSingleSizeLimit: 100 * 1024 * 1024
    });

    // 添加“添加文件”的按钮，
    //uploader.addButton({
    //    id: '#filePicker2',
    //    label: '继续添加'
    //});

    // 当有文件添加进来时执行，负责view的创建
    function addFile(file) {


        var $li = $('<div class="row" style="margin:10px;"><div class="col-sm-6">' + file.name + '</div><div class="col-sm-2"><a href="../../../ResourceFile/DownloadFile?keyValue=-1&filename=' + encodeURI(file.name) + '&recId=' + keyValue + '\" target="_blank" style="cursor:pointer"  title="下载文件"><i class="fa fa-download"></i></a>&nbsp;&nbsp;&nbsp;<a style="cursor:pointer" onclick="removeFile(\'' + file.name + '\',\'' + keyValue + '\',this)"><i class="fa fa-trash-o"></i></a></div></div>' +
                '<p class="imgWrap"></p>' +
                '<p class="progress" style="display:none;"><span></span></p>'),


            $prgress = $li.find('p.progress span'),
            $wrap = $li.find('p.imgWrap'),
            $info = $('<p class="error"></p>');
        showError = function (code) {
            switch (code) {
                case 'exceed_size':
                    text = '文件大小超出';
                    break;

                case 'interrupt':
                    text = '上传暂停';
                    break;

                default:
                    text = '上传失败，请重试';
                    break;
            }

            $info.text(text).appendTo($li);
        };

        if (file.getStatus() === 'invalid') {
            showError(file.statusText);
        } else {
            //$wrap.text('预览中');
            uploader.makeThumb(file, function (error, src) {
                if (error) {
                    //$wrap.text('不能预览');
                    return;
                }

                var img = $('<img src="' + src + '">');
                $wrap.empty().append(img);
            }, thumbnailWidth, thumbnailHeight);

            percentages[file.id] = [file.size, 0];
            file.rotation = 0;
        }

        file.on('statuschange', function (cur, prev) {
            if (prev === 'progress') {
                $prgress.hide().width(0);
            } else if (prev === 'queued') {
                $li.off('mouseenter mouseleave');

            }

            // 成功
            if (cur === 'error' || cur === 'invalid') {
                console.log(file.statusText);
                showError(file.statusText);
                percentages[file.id][1] = 1;
            } else if (cur === 'interrupt') {
                showError('interrupt');
            } else if (cur === 'queued') {
                percentages[file.id][1] = 0;
            } else if (cur === 'progress') {
                $info.remove();
                $prgress.css('display', 'block');
            } else if (cur === 'complete') {
                $li.append('<span class="success"></span>');
            }

            $li.removeClass('state-' + prev).addClass('state-' + cur);
        });

        $li.appendTo($queue);
    }



    function updateTotalProgress() {
        var loaded = 0,
            total = 0,
            spans = $progress.children(),
            percent;

        $.each(percentages, function (k, v) {
            total += v[0];
            loaded += v[0] * v[1];
        });

        percent = total ? loaded / total : 0;

        spans.eq(0).text(Math.round(percent * 100) + '%');
        spans.eq(1).css('width', Math.round(percent * 100) + '%');
        updateStatus();
    }

    function updateStatus() {
        var text = '', stats;

        //if (state === 'ready') {
        //    text = '选中' + fileCount + '个文件，共' +
        //            WebUploader.formatSize(fileSize) + '。';
        //} else if (state === 'confirm') {
        stats = uploader.getStats();
        //    if (stats.uploadFailNum) {
        //        text = '已成功上传' + stats.successNum + '个文件，' +
        //            stats.uploadFailNum + '个文件上传失败，<a class="retry" href="#">重新上传</a>'
        //    }

        //} else {
        stats = uploader.getStats();
        //    text = '共' + fileCount + '个（' +
        //            WebUploader.formatSize(fileSize) +
        //            '），已上传' + stats.successNum + '个';

        //    if (stats.uploadFailNum) {
        //        text += '，失败' + stats.uploadFailNum + '个';
        //    }
        //}

        //$info.html(text);
    }

    function setState(val) {
        var file, stats;

        if (val === state) {
            return;
        }



        state = val;

        switch (state) {
            case 'pedding':
                $placeHolder.removeClass('element-invisible');
                $queue.parent().removeClass('filled');
                $queue.hide();
                $statusBar.addClass('element-invisible');
                uploader.refresh();
                break;

            case 'ready':
                $placeHolder.addClass('element-invisible');

                $queue.parent().addClass('filled');
                $queue.show();
                $statusBar.removeClass('element-invisible');
                uploader.refresh();
                break;

            case 'uploading':

                $progress.show();

                break;

            case 'paused':
                $progress.show();
                break;

            case 'confirm':
                $progress.hide();
                stats = uploader.getStats();
                if (stats.successNum && !stats.uploadFailNum) {
                    setState('finish');
                    return;
                }
                break;
            case 'finish':
                stats = uploader.getStats();
                if (stats.successNum) {
                    dialogMsg('上传成功！', 0);
                } else {
                    // 没有成功的图片，重设
                    state = 'done';
                    location.reload();
                }
                break;
        }

        updateStatus();
    }
    uploader.onBeforeFileQueued = function (file) {


    }
    uploader.onUploadProgress = function (file, percentage) {
        var $li = $('#' + file.id),
            $percent = $li.find('.progress span');

        $percent.css('width', percentage * 100 + '%');
        percentages[file.id][1] = percentage;
        updateTotalProgress();
    };

    uploader.onFileQueued = function (file) {
        fileCount++;
        fileSize += file.size;

        if (fileCount === 1) {
            //$placeHolder.addClass('element-invisible');
            $statusBar.show();
        }

        addFile(file);
        setState('ready');
        updateTotalProgress();
    };

    uploader.onFileDequeued = function (file) {
        fileCount--;
        fileSize -= file.size;

        if (!fileCount) {
            setState('pedding');
        }
        //addFile(file);
        removeFile(file);
        updateTotalProgress();

    };

    uploader.on('all', function (type) {
        var stats;
        switch (type) {
            case 'uploadFinished':
                uploader.reset();
                setState('confirm');
                break;

            case 'startUpload':
                setState('uploading');
                break;

            case 'stopUpload':
                setState('paused');
                break;

        }
    });

    uploader.onError = function (code) {
        if (code == "Q_EXCEED_NUM_LIMIT") {
            dialogMsg("对不起，上传失败，超过限制数量！", 0);
        } else
            if (code == "Q_TYPE_DENIED") {
                dialogMsg("对不起，上传失败，请检查文件格式！", 0);
            } else if (code == "F_EXCEED_SIZE") {
                dialogMsg("对不起，上传失败，请检查文件大小！", 0);
            } else {
                dialogMsg("对不起，上传失败，请稍后再试！", 0);
            }


    };



    $info.on('click', '.retry', function () {
        uploader.retry();
    });

    $info.on('click', '.ignore', function () {
        alert('todo');
    });

    updateTotalProgress();
});




jQuery(function () {
    var $ = jQuery,    // just in case. Make sure it's not an other libaray.

        $wrap = $('#uploader1'),

        // 图片容器
        $queue = $('<div class="filelist"></div>')
            .appendTo($wrap.find('.queueList')),

        // 状态栏，包括进度和控制按钮
        $statusBar = $wrap.find('.statusBar'),

        // 文件总体选择信息。
        $info = $statusBar.find('.info'),



        // 没选择文件之前的内容。
        $placeHolder = $wrap.find('.placeholder'),

        // 总体进度条
        $progress = $statusBar.find('.progress').hide(),

        // 添加的文件数量
        fileCount = 0,

        // 添加的文件总大小
        fileSize = 0,

        // 优化retina, 在retina下这个值是2
        ratio = window.devicePixelRatio || 1,

        // 缩略图大小
        thumbnailWidth = 110 * ratio,
        thumbnailHeight = 110 * ratio,

        // 可能有pedding, ready, uploading, confirm, done.
        state = 'pedding',

        // 所有文件的进度信息，key为file id
        percentages = {},

        supportTransition = (function () {
            var s = document.createElement('p').style,
                r = 'transition' in s ||
                      'WebkitTransition' in s ||
                      'MozTransition' in s ||
                      'msTransition' in s ||
                      'OTransition' in s;
            s = null;
            return r;
        })(),

        // WebUploader实例
        uploader;

    if (!WebUploader.Uploader.support()) {
        alert('Web Uploader 不支持您的浏览器！如果你使用的是IE浏览器，请尝试升级 flash 播放器');
        throw new Error('WebUploader does not support the browser you are using.');
    }

    // 实例化
    uploader = WebUploader.create({
        auto: true,
        pick: {
            id: '#filePicker1',
            innerHTML: '选择文件'
        },
        dnd: '#uploader1 .queueList',
        paste: document.body,

        accept: {
            title: '常用文档',
            extensions: 'mp4,doc,docx,txt,pdf,ppt,xls,xlsx,jpg,png',
            mimeTypes: '*/*'
        },

        // swf文件路径
        swf: '/../content/scripts/plugins/webuploader/Uploader.swf',

        disableGlobalDnd: true,
        chunked: false,
        server: '../../../Utility/PostFileActity?recId=' + keyValue + "&filePath=Safetyday&ptype=1&isDate=1",
        fileNumLimit: 5,
        fileSizeLimit: 500 * 1024 * 1024,
        fileSingleSizeLimit: 100 * 1024 * 1024
    });

    // 添加“添加文件”的按钮，
    //uploader.addButton({
    //    id: '#filePicker2',
    //    label: '继续添加'
    //});

    // 当有文件添加进来时执行，负责view的创建
    function addFile(file) {

        var $li = $('<div class="row" style="margin:10px;"><div class="col-sm-6">' + file.name + '</div><div class="col-sm-2"><a href="../../../ResourceFile/DownloadFile?keyValue=-1&filename=' + encodeURI(file.name) + '&recId=' + keyValue + '\" target="_blank" style="cursor:pointer"  title="下载文件"><i class="fa fa-download"></i></a>&nbsp;&nbsp;&nbsp;<a style="cursor:pointer" onclick="removeFile(\'' + file.name + '\',\'' + keyValue + '\',this)"><i class="fa fa-trash-o"></i></a></div></div>' +
                '<p class="imgWrap"></p>' +
                '<p class="progress" style="display:none;"><span></span></p>'),


            $prgress = $li.find('p.progress span'),
            $wrap = $li.find('p.imgWrap'),
            $info = $('<p class="error"></p>');
        showError = function (code) {
            switch (code) {
                case 'exceed_size':
                    text = '文件大小超出';
                    break;

                case 'interrupt':
                    text = '上传暂停';
                    break;

                default:
                    text = '上传失败，请重试';
                    break;
            }

            $info.text(text).appendTo($li);
        };

        if (file.getStatus() === 'invalid') {
            showError(file.statusText);
        } else {
            //$wrap.text('预览中');
            uploader.makeThumb(file, function (error, src) {
                if (error) {
                    //$wrap.text('不能预览');
                    return;
                }

                var img = $('<img src="' + src + '">');
                $wrap.empty().append(img);
            }, thumbnailWidth, thumbnailHeight);

            percentages[file.id] = [file.size, 0];
            file.rotation = 0;
        }

        file.on('statuschange', function (cur, prev) {
            if (prev === 'progress') {
                $prgress.hide().width(0);
            } else if (prev === 'queued') {
                $li.off('mouseenter mouseleave');

            }

            // 成功
            if (cur === 'error' || cur === 'invalid') {
                console.log(file.statusText);
                showError(file.statusText);
                percentages[file.id][1] = 1;
            } else if (cur === 'interrupt') {
                showError('interrupt');
            } else if (cur === 'queued') {
                percentages[file.id][1] = 0;
            } else if (cur === 'progress') {
                $info.remove();
                $prgress.css('display', 'block');
            } else if (cur === 'complete') {
                $li.append('<span class="success"></span>');
            }

            $li.removeClass('state-' + prev).addClass('state-' + cur);
        });

        $li.appendTo($queue);
    }



    function updateTotalProgress() {
        var loaded = 0,
            total = 0,
            spans = $progress.children(),
            percent;

        $.each(percentages, function (k, v) {
            total += v[0];
            loaded += v[0] * v[1];
        });

        percent = total ? loaded / total : 0;

        spans.eq(0).text(Math.round(percent * 100) + '%');
        spans.eq(1).css('width', Math.round(percent * 100) + '%');
        updateStatus();
    }

    function updateStatus() {
        var text = '', stats;

        //if (state === 'ready') {
        //    text = '选中' + fileCount + '个文件，共' +
        //            WebUploader.formatSize(fileSize) + '。';
        //} else if (state === 'confirm') {
        stats = uploader.getStats();
        //    if (stats.uploadFailNum) {
        //        text = '已成功上传' + stats.successNum + '个文件，' +
        //            stats.uploadFailNum + '个文件上传失败，<a class="retry" href="#">重新上传</a>'
        //    }

        //} else {
        stats = uploader.getStats();
        //    text = '共' + fileCount + '个（' +
        //            WebUploader.formatSize(fileSize) +
        //            '），已上传' + stats.successNum + '个';

        //    if (stats.uploadFailNum) {
        //        text += '，失败' + stats.uploadFailNum + '个';
        //    }
        //}

        //$info.html(text);
    }

    function setState(val) {
        var file, stats;

        if (val === state) {
            return;
        }



        state = val;

        switch (state) {
            case 'pedding':
                $placeHolder.removeClass('element-invisible');
                $queue.parent().removeClass('filled');
                $queue.hide();
                $statusBar.addClass('element-invisible');
                uploader.refresh();
                break;

            case 'ready':
                $placeHolder.addClass('element-invisible');

                $queue.parent().addClass('filled');
                $queue.show();
                $statusBar.removeClass('element-invisible');
                uploader.refresh();
                break;

            case 'uploading':

                $progress.show();

                break;

            case 'paused':
                $progress.show();
                break;

            case 'confirm':
                $progress.hide();
                stats = uploader.getStats();
                if (stats.successNum && !stats.uploadFailNum) {
                    setState('finish');
                    return;
                }
                break;
            case 'finish':
                stats = uploader.getStats();
                if (stats.successNum) {
                    dialogMsg('上传成功！', 0);
                } else {
                    // 没有成功的图片，重设
                    state = 'done';
                    location.reload();
                }
                break;
        }

        updateStatus();
    }
    uploader.onBeforeFileQueued = function (file) {


    }
    uploader.onUploadProgress = function (file, percentage) {
        var $li = $('#' + file.id),
            $percent = $li.find('.progress span');

        $percent.css('width', percentage * 100 + '%');
        percentages[file.id][1] = percentage;
        updateTotalProgress();
    };

    uploader.onFileQueued = function (file) {
        fileCount++;
        fileSize += file.size;

        if (fileCount === 1) {
            //$placeHolder.addClass('element-invisible');
            $statusBar.show();
        }

        addFile(file);
        setState('ready');
        updateTotalProgress();
    };

    uploader.onFileDequeued = function (file) {
        fileCount--;
        fileSize -= file.size;

        if (!fileCount) {
            setState('pedding');
        }
        //addFile(file);
        removeFile(file);
        updateTotalProgress();

    };

    uploader.on('all', function (type) {
        var stats;
        switch (type) {
            case 'uploadFinished':
                uploader.reset();
                setState('confirm');
                break;

            case 'startUpload':
                setState('uploading');
                break;

            case 'stopUpload':
                setState('paused');
                break;

        }
    });

    uploader.onError = function (code) {
        if (code == "Q_EXCEED_NUM_LIMIT") {
            dialogMsg("对不起，上传失败，超过限制数量！", 0);
        } else
            if (code == "Q_TYPE_DENIED") {
                dialogMsg("对不起，上传失败，请检查文件格式！", 0);
            } else if (code == "F_EXCEED_SIZE") {
                dialogMsg("对不起，上传失败，请检查文件大小！", 0);
            } else {
                dialogMsg("对不起，上传失败，请稍后再试！", 0);
            }


    };



    $info.on('click', '.retry', function () {
        uploader.retry();
    });

    $info.on('click', '.ignore', function () {
        alert('todo');
    });

    updateTotalProgress();
});



jQuery(function () {
    var $ = jQuery,    // just in case. Make sure it's not an other libaray.

        $wrap = $('#uploader2'),

        // 图片容器
        $queue = $('<div class="filelist"></div>')
            .appendTo($wrap.find('.queueList')),

        // 状态栏，包括进度和控制按钮
        $statusBar = $wrap.find('.statusBar'),

        // 文件总体选择信息。
        $info = $statusBar.find('.info'),



        // 没选择文件之前的内容。
        $placeHolder = $wrap.find('.placeholder'),

        // 总体进度条
        $progress = $statusBar.find('.progress').hide(),

        // 添加的文件数量
        fileCount = 0,

        // 添加的文件总大小
        fileSize = 0,

        // 优化retina, 在retina下这个值是2
        ratio = window.devicePixelRatio || 1,

        // 缩略图大小
        thumbnailWidth = 110 * ratio,
        thumbnailHeight = 110 * ratio,

        // 可能有pedding, ready, uploading, confirm, done.
        state = 'pedding',

        // 所有文件的进度信息，key为file id
        percentages = {},

        supportTransition = (function () {
            var s = document.createElement('p').style,
                r = 'transition' in s ||
                      'WebkitTransition' in s ||
                      'MozTransition' in s ||
                      'msTransition' in s ||
                      'OTransition' in s;
            s = null;
            return r;
        })(),

        // WebUploader实例
        uploader;

    if (!WebUploader.Uploader.support()) {
        alert('Web Uploader 不支持您的浏览器！如果你使用的是IE浏览器，请尝试升级 flash 播放器');
        throw new Error('WebUploader does not support the browser you are using.');
    }

    // 实例化
    uploader = WebUploader.create({
        auto: true,
        pick: {
            id: '#filePicker2',
            innerHTML: '选择文件'
        },
        dnd: '#uploader2 .queueList',
        paste: document.body,

        accept: {
            title: '常用文档',
            extensions: 'mp4,doc,docx,txt,pdf,ppt,xls,xlsx,jpg,png',
            mimeTypes: '*/*'
        },

        // swf文件路径
        swf: '/../content/scripts/plugins/webuploader/Uploader.swf',

        disableGlobalDnd: true,
        chunked: false,
        server: '../../../Utility/PostFileActity?recId=' + keyValue + "&filePath=Safetyday&ptype=2&isDate=1",
        fileNumLimit: 5,
        fileSizeLimit: 500 * 1024 * 1024,
        fileSingleSizeLimit: 100 * 1024 * 1024
    });

    // 添加“添加文件”的按钮，
    //uploader.addButton({
    //    id: '#filePicker2',
    //    label: '继续添加'
    //});

    // 当有文件添加进来时执行，负责view的创建
    function addFile(file) {

        var $li = $('<div class="row" style="margin:10px;"><div class="col-sm-6">' + file.name + '</div><div class="col-sm-2"><a href="../../../ResourceFile/DownloadFile?keyValue=-1&filename=' + encodeURI(file.name) + '&recId=' + keyValue + '\" target="_blank" style="cursor:pointer"  title="下载文件"><i class="fa fa-download"></i></a>&nbsp;&nbsp;&nbsp;<a style="cursor:pointer" onclick="removeFile(\'' + file.name + '\',\'' + keyValue + '\',this)"><i class="fa fa-trash-o"></i></a></div></div>' +
                '<p class="imgWrap"></p>' +
                '<p class="progress" style="display:none;"><span></span></p>'),


            $prgress = $li.find('p.progress span'),
            $wrap = $li.find('p.imgWrap'),
            $info = $('<p class="error"></p>');
        showError = function (code) {
            switch (code) {
                case 'exceed_size':
                    text = '文件大小超出';
                    break;

                case 'interrupt':
                    text = '上传暂停';
                    break;

                default:
                    text = '上传失败，请重试';
                    break;
            }

            $info.text(text).appendTo($li);
        };

        if (file.getStatus() === 'invalid') {
            showError(file.statusText);
        } else {
            //$wrap.text('预览中');
            uploader.makeThumb(file, function (error, src) {
                if (error) {
                    //$wrap.text('不能预览');
                    return;
                }

                var img = $('<img src="' + src + '">');
                $wrap.empty().append(img);
            }, thumbnailWidth, thumbnailHeight);

            percentages[file.id] = [file.size, 0];
            file.rotation = 0;
        }

        file.on('statuschange', function (cur, prev) {
            if (prev === 'progress') {
                $prgress.hide().width(0);
            } else if (prev === 'queued') {
                $li.off('mouseenter mouseleave');

            }

            // 成功
            if (cur === 'error' || cur === 'invalid') {
                console.log(file.statusText);
                showError(file.statusText);
                percentages[file.id][1] = 1;
            } else if (cur === 'interrupt') {
                showError('interrupt');
            } else if (cur === 'queued') {
                percentages[file.id][1] = 0;
            } else if (cur === 'progress') {
                $info.remove();
                $prgress.css('display', 'block');
            } else if (cur === 'complete') {
                $li.append('<span class="success"></span>');
            }

            $li.removeClass('state-' + prev).addClass('state-' + cur);
        });

        $li.appendTo($queue);
    }



    function updateTotalProgress() {
        var loaded = 0,
            total = 0,
            spans = $progress.children(),
            percent;

        $.each(percentages, function (k, v) {
            total += v[0];
            loaded += v[0] * v[1];
        });

        percent = total ? loaded / total : 0;

        spans.eq(0).text(Math.round(percent * 100) + '%');
        spans.eq(1).css('width', Math.round(percent * 100) + '%');
        updateStatus();
    }

    function updateStatus() {
        var text = '', stats;

        //if (state === 'ready') {
        //    text = '选中' + fileCount + '个文件，共' +
        //            WebUploader.formatSize(fileSize) + '。';
        //} else if (state === 'confirm') {
        stats = uploader.getStats();
        //    if (stats.uploadFailNum) {
        //        text = '已成功上传' + stats.successNum + '个文件，' +
        //            stats.uploadFailNum + '个文件上传失败，<a class="retry" href="#">重新上传</a>'
        //    }

        //} else {
        stats = uploader.getStats();
        //    text = '共' + fileCount + '个（' +
        //            WebUploader.formatSize(fileSize) +
        //            '），已上传' + stats.successNum + '个';

        //    if (stats.uploadFailNum) {
        //        text += '，失败' + stats.uploadFailNum + '个';
        //    }
        //}

        //$info.html(text);
    }

    function setState(val) {
        var file, stats;

        if (val === state) {
            return;
        }



        state = val;

        switch (state) {
            case 'pedding':
                $placeHolder.removeClass('element-invisible');
                $queue.parent().removeClass('filled');
                $queue.hide();
                $statusBar.addClass('element-invisible');
                uploader.refresh();
                break;

            case 'ready':
                $placeHolder.addClass('element-invisible');

                $queue.parent().addClass('filled');
                $queue.show();
                $statusBar.removeClass('element-invisible');
                uploader.refresh();
                break;

            case 'uploading':

                $progress.show();

                break;

            case 'paused':
                $progress.show();
                break;

            case 'confirm':
                $progress.hide();
                stats = uploader.getStats();
                if (stats.successNum && !stats.uploadFailNum) {
                    setState('finish');
                    return;
                }
                break;
            case 'finish':
                stats = uploader.getStats();
                if (stats.successNum) {
                    dialogMsg('上传成功！', 0);
                } else {
                    // 没有成功的图片，重设
                    state = 'done';
                    location.reload();
                }
                break;
        }

        updateStatus();
    }
    uploader.onBeforeFileQueued = function (file) {


    }
    uploader.onUploadProgress = function (file, percentage) {
        var $li = $('#' + file.id),
            $percent = $li.find('.progress span');

        $percent.css('width', percentage * 100 + '%');
        percentages[file.id][1] = percentage;
        updateTotalProgress();
    };

    uploader.onFileQueued = function (file) {
        fileCount++;
        fileSize += file.size;

        if (fileCount === 1) {
            //$placeHolder.addClass('element-invisible');
            $statusBar.show();
        }

        addFile(file);
        setState('ready');
        updateTotalProgress();
    };

    uploader.onFileDequeued = function (file) {
        fileCount--;
        fileSize -= file.size;

        if (!fileCount) {
            setState('pedding');
        }
        //addFile(file);
        removeFile(file);
        updateTotalProgress();

    };

    uploader.on('all', function (type) {
        var stats;
        switch (type) {
            case 'uploadFinished':
                uploader.reset();
                setState('confirm');
                break;

            case 'startUpload':
                setState('uploading');
                break;

            case 'stopUpload':
                setState('paused');
                break;

        }
    });

    uploader.onError = function (code) {
        if (code == "Q_EXCEED_NUM_LIMIT") {
            dialogMsg("对不起，上传失败，超过限制数量！", 0);
        } else
            if (code == "Q_TYPE_DENIED") {
                dialogMsg("对不起，上传失败，请检查文件格式！", 0);
            } else if (code == "F_EXCEED_SIZE") {
                dialogMsg("对不起，上传失败，请检查文件大小！", 0);
            } else {
                dialogMsg("对不起，上传失败，请稍后再试！", 0);
            }


    };



    $info.on('click', '.retry', function () {
        uploader.retry();
    });

    $info.on('click', '.ignore', function () {
        alert('todo');
    });

    updateTotalProgress();
});


jQuery(function () {
    var $ = jQuery,    // just in case. Make sure it's not an other libaray.

        $wrap = $('#uploader3'),

        // 图片容器
        $queue = $('<div class="filelist"></div>')
            .appendTo($wrap.find('.queueList')),

        // 状态栏，包括进度和控制按钮
        $statusBar = $wrap.find('.statusBar'),

        // 文件总体选择信息。
        $info = $statusBar.find('.info'),



        // 没选择文件之前的内容。
        $placeHolder = $wrap.find('.placeholder'),

        // 总体进度条
        $progress = $statusBar.find('.progress').hide(),

        // 添加的文件数量
        fileCount = 0,

        // 添加的文件总大小
        fileSize = 0,

        // 优化retina, 在retina下这个值是2
        ratio = window.devicePixelRatio || 1,

        // 缩略图大小
        thumbnailWidth = 110 * ratio,
        thumbnailHeight = 110 * ratio,

        // 可能有pedding, ready, uploading, confirm, done.
        state = 'pedding',

        // 所有文件的进度信息，key为file id
        percentages = {},

        supportTransition = (function () {
            var s = document.createElement('p').style,
                r = 'transition' in s ||
                      'WebkitTransition' in s ||
                      'MozTransition' in s ||
                      'msTransition' in s ||
                      'OTransition' in s;
            s = null;
            return r;
        })(),

        // WebUploader实例
        uploader;

    if (!WebUploader.Uploader.support()) {
        alert('Web Uploader 不支持您的浏览器！如果你使用的是IE浏览器，请尝试升级 flash 播放器');
        throw new Error('WebUploader does not support the browser you are using.');
    }

    // 实例化
    uploader = WebUploader.create({
        auto: true,
        pick: {
            id: '#filePicker3',
            innerHTML: '选择文件'
        },
        dnd: '#uploader3 .queueList',
        paste: document.body,

        accept: {
            title: '常用文档',
            extensions: 'mp4,doc,docx,txt,pdf,ppt,xls,xlsx,jpg,png',
            mimeTypes: '*/*'
        },

        // swf文件路径
        swf: '/../content/scripts/plugins/webuploader/Uploader.swf',

        disableGlobalDnd: true,
        chunked: false,
        server: '../../../Utility/PostFileActity?recId=' + keyValue + "&filePath=Safetyday&ptype=3&isDate=1",
        fileNumLimit: 5,
        fileSizeLimit: 500 * 1024 * 1024,
        fileSingleSizeLimit: 100 * 1024 * 1024
    });

    // 添加“添加文件”的按钮，
    //uploader.addButton({
    //    id: '#filePicker2',
    //    label: '继续添加'
    //});

    // 当有文件添加进来时执行，负责view的创建
    function addFile(file) {

        var $li = $('<div class="row" style="margin:10px;"><div class="col-sm-6">' + file.name + '</div><div class="col-sm-2"><a href="../../../ResourceFile/DownloadFile?keyValue=-1&filename=' + encodeURI(file.name) + '&recId=' + keyValue + '\" target="_blank" style="cursor:pointer"  title="下载文件"><i class="fa fa-download"></i></a>&nbsp;&nbsp;&nbsp;<a style="cursor:pointer" onclick="removeFile(\'' + file.name + '\',\'' + keyValue + '\',this)"><i class="fa fa-trash-o"></i></a></div></div>' +
                '<p class="imgWrap"></p>' +
                '<p class="progress" style="display:none;"><span></span></p>'),


            $prgress = $li.find('p.progress span'),
            $wrap = $li.find('p.imgWrap'),
            $info = $('<p class="error"></p>');
        showError = function (code) {
            switch (code) {
                case 'exceed_size':
                    text = '文件大小超出';
                    break;

                case 'interrupt':
                    text = '上传暂停';
                    break;

                default:
                    text = '上传失败，请重试';
                    break;
            }

            $info.text(text).appendTo($li);
        };

        if (file.getStatus() === 'invalid') {
            showError(file.statusText);
        } else {
            //$wrap.text('预览中');
            uploader.makeThumb(file, function (error, src) {
                if (error) {
                    //$wrap.text('不能预览');
                    return;
                }

                var img = $('<img src="' + src + '">');
                $wrap.empty().append(img);
            }, thumbnailWidth, thumbnailHeight);

            percentages[file.id] = [file.size, 0];
            file.rotation = 0;
        }

        file.on('statuschange', function (cur, prev) {
            if (prev === 'progress') {
                $prgress.hide().width(0);
            } else if (prev === 'queued') {
                $li.off('mouseenter mouseleave');

            }

            // 成功
            if (cur === 'error' || cur === 'invalid') {
                console.log(file.statusText);
                showError(file.statusText);
                percentages[file.id][1] = 1;
            } else if (cur === 'interrupt') {
                showError('interrupt');
            } else if (cur === 'queued') {
                percentages[file.id][1] = 0;
            } else if (cur === 'progress') {
                $info.remove();
                $prgress.css('display', 'block');
            } else if (cur === 'complete') {
                $li.append('<span class="success"></span>');
            }

            $li.removeClass('state-' + prev).addClass('state-' + cur);
        });

        $li.appendTo($queue);
    }



    function updateTotalProgress() {
        var loaded = 0,
            total = 0,
            spans = $progress.children(),
            percent;

        $.each(percentages, function (k, v) {
            total += v[0];
            loaded += v[0] * v[1];
        });

        percent = total ? loaded / total : 0;

        spans.eq(0).text(Math.round(percent * 100) + '%');
        spans.eq(1).css('width', Math.round(percent * 100) + '%');
        updateStatus();
    }

    function updateStatus() {
        var text = '', stats;

        //if (state === 'ready') {
        //    text = '选中' + fileCount + '个文件，共' +
        //            WebUploader.formatSize(fileSize) + '。';
        //} else if (state === 'confirm') {
        stats = uploader.getStats();
        //    if (stats.uploadFailNum) {
        //        text = '已成功上传' + stats.successNum + '个文件，' +
        //            stats.uploadFailNum + '个文件上传失败，<a class="retry" href="#">重新上传</a>'
        //    }

        //} else {
        stats = uploader.getStats();
        //    text = '共' + fileCount + '个（' +
        //            WebUploader.formatSize(fileSize) +
        //            '），已上传' + stats.successNum + '个';

        //    if (stats.uploadFailNum) {
        //        text += '，失败' + stats.uploadFailNum + '个';
        //    }
        //}

        //$info.html(text);
    }

    function setState(val) {
        var file, stats;

        if (val === state) {
            return;
        }



        state = val;

        switch (state) {
            case 'pedding':
                $placeHolder.removeClass('element-invisible');
                $queue.parent().removeClass('filled');
                $queue.hide();
                $statusBar.addClass('element-invisible');
                uploader.refresh();
                break;

            case 'ready':
                $placeHolder.addClass('element-invisible');

                $queue.parent().addClass('filled');
                $queue.show();
                $statusBar.removeClass('element-invisible');
                uploader.refresh();
                break;

            case 'uploading':

                $progress.show();

                break;

            case 'paused':
                $progress.show();
                break;

            case 'confirm':
                $progress.hide();
                stats = uploader.getStats();
                if (stats.successNum && !stats.uploadFailNum) {
                    setState('finish');
                    return;
                }
                break;
            case 'finish':
                stats = uploader.getStats();
                if (stats.successNum) {
                    dialogMsg('上传成功！', 0);
                } else {
                    // 没有成功的图片，重设
                    state = 'done';
                    location.reload();
                }
                break;
        }

        updateStatus();
    }
    uploader.onBeforeFileQueued = function (file) {


    }
    uploader.onUploadProgress = function (file, percentage) {
        var $li = $('#' + file.id),
            $percent = $li.find('.progress span');

        $percent.css('width', percentage * 100 + '%');
        percentages[file.id][1] = percentage;
        updateTotalProgress();
    };

    uploader.onFileQueued = function (file) {
        fileCount++;
        fileSize += file.size;

        if (fileCount === 1) {
            //$placeHolder.addClass('element-invisible');
            $statusBar.show();
        }

        addFile(file);
        setState('ready');
        updateTotalProgress();
    };

    uploader.onFileDequeued = function (file) {
        fileCount--;
        fileSize -= file.size;

        if (!fileCount) {
            setState('pedding');
        }
        //addFile(file);
        removeFile(file);
        updateTotalProgress();

    };

    uploader.on('all', function (type) {
        var stats;
        switch (type) {
            case 'uploadFinished':
                uploader.reset();
                setState('confirm');
                break;

            case 'startUpload':
                setState('uploading');
                break;

            case 'stopUpload':
                setState('paused');
                break;

        }
    });

    uploader.onError = function (code) {
        if (code == "Q_EXCEED_NUM_LIMIT") {
            dialogMsg("对不起，上传失败，超过限制数量！", 0);
        } else
            if (code == "Q_TYPE_DENIED") {
                dialogMsg("对不起，上传失败，请检查文件格式！", 0);
            } else if (code == "F_EXCEED_SIZE") {
                dialogMsg("对不起，上传失败，请检查文件大小！", 0);
            } else {
                dialogMsg("对不起，上传失败，请稍后再试！", 0);
            }


    };



    $info.on('click', '.retry', function () {
        uploader.retry();
    });

    $info.on('click', '.ignore', function () {
        alert('todo');
    });

    updateTotalProgress();
});