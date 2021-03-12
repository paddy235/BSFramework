


var keyValue = $("#BaseId").val();


jQuery(function () {
    var $ = jQuery,    // just in case. Make sure it's not an other libaray.

        $wrap = $('#uploader'),

        // ͼƬ����
        $queue = $('<div class="filelist"></div>')
            .appendTo($wrap.find('.queueList')),

        // ״̬�����������ȺͿ��ư�ť
        $statusBar = $wrap.find('.statusBar'),

        // �ļ�����ѡ����Ϣ��
        $info = $statusBar.find('.info'),



        // ûѡ���ļ�֮ǰ�����ݡ�
        $placeHolder = $wrap.find('.placeholder'),

        // ���������
        $progress = $statusBar.find('.progress').hide(),

        // ��ӵ��ļ�����
        fileCount = 0,

        // ��ӵ��ļ��ܴ�С
        fileSize = 0,

        // �Ż�retina, ��retina�����ֵ��2
        ratio = window.devicePixelRatio || 1,

        // ����ͼ��С
        thumbnailWidth = 110 * ratio,
        thumbnailHeight = 110 * ratio,

        // ������pedding, ready, uploading, confirm, done.
        state = 'pedding',

        // �����ļ��Ľ�����Ϣ��keyΪfile id
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

        // WebUploaderʵ��
        uploader;

    if (!WebUploader.Uploader.support()) {
        alert('Web Uploader ��֧������������������ʹ�õ���IE��������볢������ flash ������');
        throw new Error('WebUploader does not support the browser you are using.');
    }

    // ʵ����
    uploader = WebUploader.create({
        auto: true,
        pick: {
            id: '#filePicker',
            innerHTML: ('���ѡ���ļ�')
        },
        dnd: '#uploader .queueList',
        paste: document.body,

        accept: {
            title: '�����ĵ�',
            extensions: 'mp4,doc,docx,txt,pdf,ppt,xls,xlsx,jpg,png',
            mimeTypes: '*/*'
        },

        // swf�ļ�·��
        swf: '/../content/scripts/plugins/webuploader/Uploader.swf',

        disableGlobalDnd: true,
        chunked: false,
        server: '../../../Utility/PostFileActity?recId=' + keyValue + "&filePath=Safetyday&ptype=0&isDate=1",
        fileNumLimit: 5,
        fileSizeLimit: 500 * 1024 * 1024,
        fileSingleSizeLimit: 100 * 1024 * 1024
    });

    // ��ӡ�����ļ����İ�ť��
    //uploader.addButton({
    //    id: '#filePicker2',
    //    label: '�������'
    //});

    // �����ļ���ӽ���ʱִ�У�����view�Ĵ���
    function addFile(file) {


        var $li = $('<div class="row" style="margin:10px;"><div class="col-sm-6">' + file.name + '</div><div class="col-sm-2"><a href="../../../ResourceFile/DownloadFile?keyValue=-1&filename=' + encodeURI(file.name) + '&recId=' + keyValue + '\" target="_blank" style="cursor:pointer"  title="�����ļ�"><i class="fa fa-download"></i></a>&nbsp;&nbsp;&nbsp;<a style="cursor:pointer" onclick="removeFile(\'' + file.name + '\',\'' + keyValue + '\',this)"><i class="fa fa-trash-o"></i></a></div></div>' +
                '<p class="imgWrap"></p>' +
                '<p class="progress" style="display:none;"><span></span></p>'),


            $prgress = $li.find('p.progress span'),
            $wrap = $li.find('p.imgWrap'),
            $info = $('<p class="error"></p>');
        showError = function (code) {
            switch (code) {
                case 'exceed_size':
                    text = '�ļ���С����';
                    break;

                case 'interrupt':
                    text = '�ϴ���ͣ';
                    break;

                default:
                    text = '�ϴ�ʧ�ܣ�������';
                    break;
            }

            $info.text(text).appendTo($li);
        };

        if (file.getStatus() === 'invalid') {
            showError(file.statusText);
        } else {
            //$wrap.text('Ԥ����');
            uploader.makeThumb(file, function (error, src) {
                if (error) {
                    //$wrap.text('����Ԥ��');
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

            // �ɹ�
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
        //    text = 'ѡ��' + fileCount + '���ļ�����' +
        //            WebUploader.formatSize(fileSize) + '��';
        //} else if (state === 'confirm') {
        stats = uploader.getStats();
        //    if (stats.uploadFailNum) {
        //        text = '�ѳɹ��ϴ�' + stats.successNum + '���ļ���' +
        //            stats.uploadFailNum + '���ļ��ϴ�ʧ�ܣ�<a class="retry" href="#">�����ϴ�</a>'
        //    }

        //} else {
        stats = uploader.getStats();
        //    text = '��' + fileCount + '����' +
        //            WebUploader.formatSize(fileSize) +
        //            '�������ϴ�' + stats.successNum + '��';

        //    if (stats.uploadFailNum) {
        //        text += '��ʧ��' + stats.uploadFailNum + '��';
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
                    dialogMsg('�ϴ��ɹ���', 0);
                } else {
                    // û�гɹ���ͼƬ������
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
            dialogMsg("�Բ����ϴ�ʧ�ܣ���������������", 0);
        } else
            if (code == "Q_TYPE_DENIED") {
                dialogMsg("�Բ����ϴ�ʧ�ܣ������ļ���ʽ��", 0);
            } else if (code == "F_EXCEED_SIZE") {
                dialogMsg("�Բ����ϴ�ʧ�ܣ������ļ���С��", 0);
            } else {
                dialogMsg("�Բ����ϴ�ʧ�ܣ����Ժ����ԣ�", 0);
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

        // ͼƬ����
        $queue = $('<div class="filelist"></div>')
            .appendTo($wrap.find('.queueList')),

        // ״̬�����������ȺͿ��ư�ť
        $statusBar = $wrap.find('.statusBar'),

        // �ļ�����ѡ����Ϣ��
        $info = $statusBar.find('.info'),



        // ûѡ���ļ�֮ǰ�����ݡ�
        $placeHolder = $wrap.find('.placeholder'),

        // ���������
        $progress = $statusBar.find('.progress').hide(),

        // ��ӵ��ļ�����
        fileCount = 0,

        // ��ӵ��ļ��ܴ�С
        fileSize = 0,

        // �Ż�retina, ��retina�����ֵ��2
        ratio = window.devicePixelRatio || 1,

        // ����ͼ��С
        thumbnailWidth = 110 * ratio,
        thumbnailHeight = 110 * ratio,

        // ������pedding, ready, uploading, confirm, done.
        state = 'pedding',

        // �����ļ��Ľ�����Ϣ��keyΪfile id
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

        // WebUploaderʵ��
        uploader;

    if (!WebUploader.Uploader.support()) {
        alert('Web Uploader ��֧������������������ʹ�õ���IE��������볢������ flash ������');
        throw new Error('WebUploader does not support the browser you are using.');
    }

    // ʵ����
    uploader = WebUploader.create({
        auto: true,
        pick: {
            id: '#filePicker1',
            innerHTML: 'ѡ���ļ�'
        },
        dnd: '#uploader1 .queueList',
        paste: document.body,

        accept: {
            title: '�����ĵ�',
            extensions: 'mp4,doc,docx,txt,pdf,ppt,xls,xlsx,jpg,png',
            mimeTypes: '*/*'
        },

        // swf�ļ�·��
        swf: '/../content/scripts/plugins/webuploader/Uploader.swf',

        disableGlobalDnd: true,
        chunked: false,
        server: '../../../Utility/PostFileActity?recId=' + keyValue + "&filePath=Safetyday&ptype=1&isDate=1",
        fileNumLimit: 5,
        fileSizeLimit: 500 * 1024 * 1024,
        fileSingleSizeLimit: 100 * 1024 * 1024
    });

    // ��ӡ�����ļ����İ�ť��
    //uploader.addButton({
    //    id: '#filePicker2',
    //    label: '�������'
    //});

    // �����ļ���ӽ���ʱִ�У�����view�Ĵ���
    function addFile(file) {

        var $li = $('<div class="row" style="margin:10px;"><div class="col-sm-6">' + file.name + '</div><div class="col-sm-2"><a href="../../../ResourceFile/DownloadFile?keyValue=-1&filename=' + encodeURI(file.name) + '&recId=' + keyValue + '\" target="_blank" style="cursor:pointer"  title="�����ļ�"><i class="fa fa-download"></i></a>&nbsp;&nbsp;&nbsp;<a style="cursor:pointer" onclick="removeFile(\'' + file.name + '\',\'' + keyValue + '\',this)"><i class="fa fa-trash-o"></i></a></div></div>' +
                '<p class="imgWrap"></p>' +
                '<p class="progress" style="display:none;"><span></span></p>'),


            $prgress = $li.find('p.progress span'),
            $wrap = $li.find('p.imgWrap'),
            $info = $('<p class="error"></p>');
        showError = function (code) {
            switch (code) {
                case 'exceed_size':
                    text = '�ļ���С����';
                    break;

                case 'interrupt':
                    text = '�ϴ���ͣ';
                    break;

                default:
                    text = '�ϴ�ʧ�ܣ�������';
                    break;
            }

            $info.text(text).appendTo($li);
        };

        if (file.getStatus() === 'invalid') {
            showError(file.statusText);
        } else {
            //$wrap.text('Ԥ����');
            uploader.makeThumb(file, function (error, src) {
                if (error) {
                    //$wrap.text('����Ԥ��');
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

            // �ɹ�
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
        //    text = 'ѡ��' + fileCount + '���ļ�����' +
        //            WebUploader.formatSize(fileSize) + '��';
        //} else if (state === 'confirm') {
        stats = uploader.getStats();
        //    if (stats.uploadFailNum) {
        //        text = '�ѳɹ��ϴ�' + stats.successNum + '���ļ���' +
        //            stats.uploadFailNum + '���ļ��ϴ�ʧ�ܣ�<a class="retry" href="#">�����ϴ�</a>'
        //    }

        //} else {
        stats = uploader.getStats();
        //    text = '��' + fileCount + '����' +
        //            WebUploader.formatSize(fileSize) +
        //            '�������ϴ�' + stats.successNum + '��';

        //    if (stats.uploadFailNum) {
        //        text += '��ʧ��' + stats.uploadFailNum + '��';
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
                    dialogMsg('�ϴ��ɹ���', 0);
                } else {
                    // û�гɹ���ͼƬ������
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
            dialogMsg("�Բ����ϴ�ʧ�ܣ���������������", 0);
        } else
            if (code == "Q_TYPE_DENIED") {
                dialogMsg("�Բ����ϴ�ʧ�ܣ������ļ���ʽ��", 0);
            } else if (code == "F_EXCEED_SIZE") {
                dialogMsg("�Բ����ϴ�ʧ�ܣ������ļ���С��", 0);
            } else {
                dialogMsg("�Բ����ϴ�ʧ�ܣ����Ժ����ԣ�", 0);
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

        // ͼƬ����
        $queue = $('<div class="filelist"></div>')
            .appendTo($wrap.find('.queueList')),

        // ״̬�����������ȺͿ��ư�ť
        $statusBar = $wrap.find('.statusBar'),

        // �ļ�����ѡ����Ϣ��
        $info = $statusBar.find('.info'),



        // ûѡ���ļ�֮ǰ�����ݡ�
        $placeHolder = $wrap.find('.placeholder'),

        // ���������
        $progress = $statusBar.find('.progress').hide(),

        // ��ӵ��ļ�����
        fileCount = 0,

        // ��ӵ��ļ��ܴ�С
        fileSize = 0,

        // �Ż�retina, ��retina�����ֵ��2
        ratio = window.devicePixelRatio || 1,

        // ����ͼ��С
        thumbnailWidth = 110 * ratio,
        thumbnailHeight = 110 * ratio,

        // ������pedding, ready, uploading, confirm, done.
        state = 'pedding',

        // �����ļ��Ľ�����Ϣ��keyΪfile id
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

        // WebUploaderʵ��
        uploader;

    if (!WebUploader.Uploader.support()) {
        alert('Web Uploader ��֧������������������ʹ�õ���IE��������볢������ flash ������');
        throw new Error('WebUploader does not support the browser you are using.');
    }

    // ʵ����
    uploader = WebUploader.create({
        auto: true,
        pick: {
            id: '#filePicker2',
            innerHTML: 'ѡ���ļ�'
        },
        dnd: '#uploader2 .queueList',
        paste: document.body,

        accept: {
            title: '�����ĵ�',
            extensions: 'mp4,doc,docx,txt,pdf,ppt,xls,xlsx,jpg,png',
            mimeTypes: '*/*'
        },

        // swf�ļ�·��
        swf: '/../content/scripts/plugins/webuploader/Uploader.swf',

        disableGlobalDnd: true,
        chunked: false,
        server: '../../../Utility/PostFileActity?recId=' + keyValue + "&filePath=Safetyday&ptype=2&isDate=1",
        fileNumLimit: 5,
        fileSizeLimit: 500 * 1024 * 1024,
        fileSingleSizeLimit: 100 * 1024 * 1024
    });

    // ��ӡ�����ļ����İ�ť��
    //uploader.addButton({
    //    id: '#filePicker2',
    //    label: '�������'
    //});

    // �����ļ���ӽ���ʱִ�У�����view�Ĵ���
    function addFile(file) {

        var $li = $('<div class="row" style="margin:10px;"><div class="col-sm-6">' + file.name + '</div><div class="col-sm-2"><a href="../../../ResourceFile/DownloadFile?keyValue=-1&filename=' + encodeURI(file.name) + '&recId=' + keyValue + '\" target="_blank" style="cursor:pointer"  title="�����ļ�"><i class="fa fa-download"></i></a>&nbsp;&nbsp;&nbsp;<a style="cursor:pointer" onclick="removeFile(\'' + file.name + '\',\'' + keyValue + '\',this)"><i class="fa fa-trash-o"></i></a></div></div>' +
                '<p class="imgWrap"></p>' +
                '<p class="progress" style="display:none;"><span></span></p>'),


            $prgress = $li.find('p.progress span'),
            $wrap = $li.find('p.imgWrap'),
            $info = $('<p class="error"></p>');
        showError = function (code) {
            switch (code) {
                case 'exceed_size':
                    text = '�ļ���С����';
                    break;

                case 'interrupt':
                    text = '�ϴ���ͣ';
                    break;

                default:
                    text = '�ϴ�ʧ�ܣ�������';
                    break;
            }

            $info.text(text).appendTo($li);
        };

        if (file.getStatus() === 'invalid') {
            showError(file.statusText);
        } else {
            //$wrap.text('Ԥ����');
            uploader.makeThumb(file, function (error, src) {
                if (error) {
                    //$wrap.text('����Ԥ��');
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

            // �ɹ�
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
        //    text = 'ѡ��' + fileCount + '���ļ�����' +
        //            WebUploader.formatSize(fileSize) + '��';
        //} else if (state === 'confirm') {
        stats = uploader.getStats();
        //    if (stats.uploadFailNum) {
        //        text = '�ѳɹ��ϴ�' + stats.successNum + '���ļ���' +
        //            stats.uploadFailNum + '���ļ��ϴ�ʧ�ܣ�<a class="retry" href="#">�����ϴ�</a>'
        //    }

        //} else {
        stats = uploader.getStats();
        //    text = '��' + fileCount + '����' +
        //            WebUploader.formatSize(fileSize) +
        //            '�������ϴ�' + stats.successNum + '��';

        //    if (stats.uploadFailNum) {
        //        text += '��ʧ��' + stats.uploadFailNum + '��';
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
                    dialogMsg('�ϴ��ɹ���', 0);
                } else {
                    // û�гɹ���ͼƬ������
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
            dialogMsg("�Բ����ϴ�ʧ�ܣ���������������", 0);
        } else
            if (code == "Q_TYPE_DENIED") {
                dialogMsg("�Բ����ϴ�ʧ�ܣ������ļ���ʽ��", 0);
            } else if (code == "F_EXCEED_SIZE") {
                dialogMsg("�Բ����ϴ�ʧ�ܣ������ļ���С��", 0);
            } else {
                dialogMsg("�Բ����ϴ�ʧ�ܣ����Ժ����ԣ�", 0);
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

        // ͼƬ����
        $queue = $('<div class="filelist"></div>')
            .appendTo($wrap.find('.queueList')),

        // ״̬�����������ȺͿ��ư�ť
        $statusBar = $wrap.find('.statusBar'),

        // �ļ�����ѡ����Ϣ��
        $info = $statusBar.find('.info'),



        // ûѡ���ļ�֮ǰ�����ݡ�
        $placeHolder = $wrap.find('.placeholder'),

        // ���������
        $progress = $statusBar.find('.progress').hide(),

        // ��ӵ��ļ�����
        fileCount = 0,

        // ��ӵ��ļ��ܴ�С
        fileSize = 0,

        // �Ż�retina, ��retina�����ֵ��2
        ratio = window.devicePixelRatio || 1,

        // ����ͼ��С
        thumbnailWidth = 110 * ratio,
        thumbnailHeight = 110 * ratio,

        // ������pedding, ready, uploading, confirm, done.
        state = 'pedding',

        // �����ļ��Ľ�����Ϣ��keyΪfile id
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

        // WebUploaderʵ��
        uploader;

    if (!WebUploader.Uploader.support()) {
        alert('Web Uploader ��֧������������������ʹ�õ���IE��������볢������ flash ������');
        throw new Error('WebUploader does not support the browser you are using.');
    }

    // ʵ����
    uploader = WebUploader.create({
        auto: true,
        pick: {
            id: '#filePicker3',
            innerHTML: 'ѡ���ļ�'
        },
        dnd: '#uploader3 .queueList',
        paste: document.body,

        accept: {
            title: '�����ĵ�',
            extensions: 'mp4,doc,docx,txt,pdf,ppt,xls,xlsx,jpg,png',
            mimeTypes: '*/*'
        },

        // swf�ļ�·��
        swf: '/../content/scripts/plugins/webuploader/Uploader.swf',

        disableGlobalDnd: true,
        chunked: false,
        server: '../../../Utility/PostFileActity?recId=' + keyValue + "&filePath=Safetyday&ptype=3&isDate=1",
        fileNumLimit: 5,
        fileSizeLimit: 500 * 1024 * 1024,
        fileSingleSizeLimit: 100 * 1024 * 1024
    });

    // ��ӡ�����ļ����İ�ť��
    //uploader.addButton({
    //    id: '#filePicker2',
    //    label: '�������'
    //});

    // �����ļ���ӽ���ʱִ�У�����view�Ĵ���
    function addFile(file) {

        var $li = $('<div class="row" style="margin:10px;"><div class="col-sm-6">' + file.name + '</div><div class="col-sm-2"><a href="../../../ResourceFile/DownloadFile?keyValue=-1&filename=' + encodeURI(file.name) + '&recId=' + keyValue + '\" target="_blank" style="cursor:pointer"  title="�����ļ�"><i class="fa fa-download"></i></a>&nbsp;&nbsp;&nbsp;<a style="cursor:pointer" onclick="removeFile(\'' + file.name + '\',\'' + keyValue + '\',this)"><i class="fa fa-trash-o"></i></a></div></div>' +
                '<p class="imgWrap"></p>' +
                '<p class="progress" style="display:none;"><span></span></p>'),


            $prgress = $li.find('p.progress span'),
            $wrap = $li.find('p.imgWrap'),
            $info = $('<p class="error"></p>');
        showError = function (code) {
            switch (code) {
                case 'exceed_size':
                    text = '�ļ���С����';
                    break;

                case 'interrupt':
                    text = '�ϴ���ͣ';
                    break;

                default:
                    text = '�ϴ�ʧ�ܣ�������';
                    break;
            }

            $info.text(text).appendTo($li);
        };

        if (file.getStatus() === 'invalid') {
            showError(file.statusText);
        } else {
            //$wrap.text('Ԥ����');
            uploader.makeThumb(file, function (error, src) {
                if (error) {
                    //$wrap.text('����Ԥ��');
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

            // �ɹ�
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
        //    text = 'ѡ��' + fileCount + '���ļ�����' +
        //            WebUploader.formatSize(fileSize) + '��';
        //} else if (state === 'confirm') {
        stats = uploader.getStats();
        //    if (stats.uploadFailNum) {
        //        text = '�ѳɹ��ϴ�' + stats.successNum + '���ļ���' +
        //            stats.uploadFailNum + '���ļ��ϴ�ʧ�ܣ�<a class="retry" href="#">�����ϴ�</a>'
        //    }

        //} else {
        stats = uploader.getStats();
        //    text = '��' + fileCount + '����' +
        //            WebUploader.formatSize(fileSize) +
        //            '�������ϴ�' + stats.successNum + '��';

        //    if (stats.uploadFailNum) {
        //        text += '��ʧ��' + stats.uploadFailNum + '��';
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
                    dialogMsg('�ϴ��ɹ���', 0);
                } else {
                    // û�гɹ���ͼƬ������
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
            dialogMsg("�Բ����ϴ�ʧ�ܣ���������������", 0);
        } else
            if (code == "Q_TYPE_DENIED") {
                dialogMsg("�Բ����ϴ�ʧ�ܣ������ļ���ʽ��", 0);
            } else if (code == "F_EXCEED_SIZE") {
                dialogMsg("�Բ����ϴ�ʧ�ܣ������ļ���С��", 0);
            } else {
                dialogMsg("�Բ����ϴ�ʧ�ܣ����Ժ����ԣ�", 0);
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