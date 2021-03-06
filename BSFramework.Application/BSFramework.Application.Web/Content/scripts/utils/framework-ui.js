    var path = document.baseURI;
    path = path.replace(top.location.origin, "");
    var ind = path.indexOf('/', 2);
    path = path.substring(0, ind);

//加载提示框
function Loading(bool, text) {
    if (top.$("#loading_manage") != undefined) {
        top.$("#loading_manage").css("left", ($(top.window.document.body).width() - top.$("#loading_manage").width()) / 2 - 54);
        top.$("#loading_manage").css("top", ($(top.window.document.body).height() - top.$("#loading_manage").height()) / 2);
        var ajaxbg = top.$("#loading_background,#loading_manage");
        if (bool) {
            ajaxbg.show();
        } else {
            if (top.$("#loading_manage").attr('isTableLoading') == undefined) {
                ajaxbg.hide();
                top.$(".ajax-loader").hide();
            }
        }
        if (!!text) {
            top.$("#loading_manage").html(text);
        } else {
            top.$("#loading_manage").html("请稍候,正在努力加载中…");
        }
    }

};
//flash检测
function flashChecker() {
    var hasFlash = 0;　　　　 //是否安装了flash
    var flashVersion = 0;　　 //flash版本

    if (document.all) {
        var swf = new ActiveXObject('ShockwaveFlash.ShockwaveFlash');
        if (swf) {
            hasFlash = 1;
            VSwf = swf.GetVariable("$version");
            flashVersion = parseInt(VSwf.split(" ")[1].split(",")[0]);
        }
    } else {
        if (navigator.plugins && navigator.plugins.length > 0) {
            var swf = navigator.plugins["Shockwave Flash"];
            if (swf) {
                hasFlash = 1;
                var words = swf.description.split(" ");
                for (var i = 0; i < words.length; ++i) {
                    if (isNaN(parseInt(words[i]))) continue;
                    flashVersion = parseInt(words[i]);
                }
            }
        }
    }
    return { f: hasFlash, v: flashVersion };
};
tabiframeId = function () {
    var iframeId = top.$(".desktop_iframe:visible").attr("id");
    if (iframeId == undefined) {
        iframeId = top.$(".LRADMS_iframe:visible").attr("id");
    }
    return iframeId;
}
$.fn.ComboBox = function (options) {
    //options参数：description,height,width,allowSearch,url,param,data
    var $select = $(this);
    if (!$select.attr('id')) {
        return false;
    }
    if (options) {
        if ($select.find('.ui-select-text').length == 0) {
            var $select_html = "";
            $select_html += "<div class=\"ui-select-text\" style='color:#999;'>" + options.description + "</div>";
            $select_html += "<div class=\"ui-select-option\">";
            $select_html += "<div class=\"ui-select-option-content\" style=\"max-height: " + options.height + "\">" + $select.html() + "</div>";
            if (options.allowSearch) {
                $select_html += "<div class=\"ui-select-option-search\"><input type=\"text\" class=\"form-control\" placeholder=\"输入关键字,按Enter键搜索\" /><span class=\"input-query\" title=\"Search\"><i class=\"fa fa-search\"></i></span></div>";
            }
            $select_html += "</div>";
            $select.html('');
            $select.append($select_html);
        }
    }
    var $option_html = $($("<p>").append($select.find('.ui-select-option').clone()).html());
    $option_html.attr('id', $select.attr('id') + '-option');
    $select.find('.ui-select-option').remove();
    if ($option_html.length > 0) {
        $('body').find('#' + $select.attr('id') + '-option').remove();
    }
    $('body').prepend($option_html);
    var $option = $("#" + $select.attr('id') + "-option");
    if (options.url != undefined) {
        $option.find('.ui-select-option-content').html('');
        $.ajax({
            url: options.url,
            data: options.param,
            type: "GET",
            dataType: "json",
            async: false,
            success: function (data) {
                options.data = data;
                var json = data;
                loadComboBoxView(json);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                dialogMsg(errorThrown, -1);
            }
        });
    }
    else if (options.data != undefined) {
        var json = options.data;
        loadComboBoxView(json);
    }
    else {
        $option.find('li').css('padding', "0 5px");
        $option.find('li').click(function (e) {
            var data_text = $(this).text();
            var data_value = $(this).attr('data-value');
            $select.attr("data-value", data_value).attr("data-text", data_text);
            $select.find('.ui-select-text').html(data_text).css('color', '#000');
            $option.slideUp(150);
            $select.trigger("change");
            e.stopPropagation();
        }).hover(function (e) {
            if (!$(this).hasClass('liactive')) {
                $(this).toggleClass('on');
            }
            e.stopPropagation();
        });
    }
    function loadComboBoxView(json, searchValue, m) {
        if (json.length > 0) {
            var $_html = $('<ul></ul>');
            if (options.description) {
                $_html.append('<li data-value="">' + options.description + '</li>');
            }
            $.each(json, function (i) {
                var row = json[i];
                var title = row[options.title];
                if (title == undefined) {
                    title = "";
                }
                if (searchValue != undefined) {
                    if (row[m.text].indexOf(searchValue) != -1) {
                        $_html.append('<li data-value="' + row[options.id] + '" title="' + title + '">' + row[options.text] + '</li>');
                    }
                }
                else {
                    $_html.append('<li data-value="' + row[options.id] + '" title="' + title + '">' + row[options.text] + '</li>');
                }
            });
            $option.find('.ui-select-option-content').html($_html);
            $option.find('li').css('padding', "0 5px");
            $option.find('li').click(function (e) {
                var data_text = $(this).text();
                var data_value = $(this).attr('data-value');
                $select.attr("data-value", data_value).attr("data-text", data_text);
                $select.find('.ui-select-text').html(data_text).css('color', '#000');
                $option.slideUp(150);
                $select.trigger("change");
                e.stopPropagation();
            }).hover(function (e) {
                if (!$(this).hasClass('liactive')) {
                    $(this).toggleClass('on');
                }
                e.stopPropagation();
            });
        }
    }
    //操作搜索事件
    if (options.allowSearch) {
        $option.find('.ui-select-option-search').find('input').bind("keypress", function (e) {
            var key = window.event ? e.keyCode : e.which;
            if (key == 13) {
                var value = $(this).val();
                loadComboBoxView($(this)[0].options.data, value, $(this)[0].options);
            }
        }).focus(function () {
            $(this).select();
        })[0]["options"] = options;
    }

    $select.unbind('click');
    $select.bind("click", function (e) {
        if ($select.attr('readonly') == 'readonly' || $select.attr('disabled') == 'disabled') {
            return false;
        }
        $(this).addClass('ui-select-focus');
        if ($option.is(":hidden")) {
            $select.find('.ui-select-option').hide();
            $('.ui-select-option').hide();
            var left = $select.offset().left;
            var top = $select.offset().top + 29;
            var width = $select.width();
            if (options.width) {
                width = options.width;
            }
            if (($option.height() + top) < $(window).height()) {
                $option.slideDown(150).css({ top: top, left: left, width: width });
            } else {
                var _top = (top - $option.height() - 32)
                $option.show().css({ top: _top, left: left, width: width });
                $option.attr('data-show', true);
            }
            $option.css('border-top', '1px solid #ccc');
            $option.find('li').removeClass('liactive');
            $option.find('[data-value=' + $select.attr('data-value') + ']').addClass('liactive');
            $option.find('.ui-select-option-search').find('input').select();
        } else {
            if ($option.attr('data-show')) {
                $option.hide();
            } else {
                $option.slideUp(150);
            }
        }
        e.stopPropagation();
    });
    $(document).click(function (e) {
        var e = e ? e : window.event;
        var tar = e.srcElement || e.target;
        if (!$(tar).hasClass('form-control')) {
            if ($option.attr('data-show')) {
                $option.hide();
            } else {
                $option.slideUp(150);
            }
            $select.removeClass('ui-select-focus');
            e.stopPropagation();
        }
    });
    return $select;
}
$.fn.ComboBoxSetValue = function (value) {
    if ($.isNullOrEmpty(value)) {
        return;
    }
    var $select = $(this);
    var $option = $("#" + $select.attr('id') + "-option");
    $select.attr('data-value', value);
    var $item = $option.find('ul').find('[data-value=' + value + ']');
    var data_text = $item.html();
    var data_code = $item.attr("data-code");
    var data_dept = $item.attr("data-dept");
    if (data_code) {
        $select.attr('data-code', data_code);
    }
    if (data_dept) {
        $select.attr('data-dept', data_dept);
    }
    if (data_text) {
        $select.attr('data-text', data_text);
        $select.find('.ui-select-text').html(data_text).css('color', '#000');
        $option.find('ul').find('[data-value=' + value + ']').addClass('liactive')
    }
    return $select;
}
$.fn.ComboBoxTree = function (options) {
    //options参数：description,height,allowSearch,appendTo,click,url,param,method,icon
    var $select = $(this);
    if (!$select.attr('id')) {
        return false; ui - select - option
    }
    if ($select.find('.ui-select-text').length == 0) {
        var $select_html = "";
        $select_html += "<div class=\"ui-select-text\"  style='color:#999;'>" + options.description + "</div>";
        $select_html += "<div class=\"ui-select-option\">";
        $select_html += "<div class=\"ui-select-option-content\" style=\"max-height: " + options.height + "\"></div>";
        if (options.allowSearch) {
            $select_html += "<div class=\"ui-select-option-search\"><input type=\"text\" class=\"form-control\" placeholder=\"输入关键字,按Enter键搜索\" /><span class=\"input-query\" title=\"Search\"><i class=\"fa fa-search\" title=\"按回车查询\"></i></span></div>";
        }
        $select_html += "</div>";
        $select.append($select_html);
    }


    var $option_html = $($("<p>").append($select.find('.ui-select-option').clone()).html());
    $option_html.attr('id', $select.attr('id') + '-option');
    $select.find('.ui-select-option').remove();
    if (options.appendTo) {
        $(options.appendTo).prepend($option_html);
    } else {
        $('body').prepend($option_html);
    }
    var $option = $("#" + $select.attr('id') + "-option");
    var $option_content = $("#" + $select.attr('id') + "-option").find('.ui-select-option-content');
    loadtreeview(options.url);
    function loadtreeview(url) {
        $option_content.treeview({
            onnodeclick: function (item) {
                $select.attr("data-value", item.id).attr("data-text", item.text).attr("data-code", item.Code);
                if (item.IsOrg != undefined) {
                    $select.attr("data-isorg", item.IsOrg)
                }
                if (item.Dept != undefined) {
                    $select.attr("data-dept", item.Dept)
                }
                $select.find('.ui-select-text').html(item.text).css('color', '#000');
                $option.find('.ui-select-option-search').find('input').val("");
                $select.trigger("change");
                if (options.click) {
                    options.click(item);
                }
            },
            height: options.height,
            url: url,
            param: options.param,
            method: options.method,
            description: options.description
        });
    }
    if (options.allowSearch) {
        $option.find('.ui-select-option-search').find('input').attr('data-url', options.url);
        $option.find('.ui-select-option-search').find('input').bind("keypress", function (e) {
            var key = window.event ? e.keyCode : e.which;
            if (key == 13) {
                var value = $(this).val();
                var url = changeUrlParam($option.find('.ui-select-option-search').find('input').attr('data-url'), "keyword", escape(value));
                loadtreeview(url);
            }
        }).focus(function () {
            $(this).select();
        });
    }
    if (options.icon) {
        $option.find('i').remove();
        $option.find('img').remove();
    }
    $select.find('.ui-select-text').unbind('click');
    $select.find('.ui-select-text').bind("click", function (e) {
        if ($select.attr('readonly') == 'readonly' || $select.attr('disabled') == 'disabled') {
            return false;
        }
        $(this).parent().addClass('ui-select-focus');
        if ($option.is(":hidden")) {
            $select.find('.ui-select-option').hide();
            $('.ui-select-option').hide();
            var left = $select.offset().left;
            var top = $select.offset().top + 29;
            var width = $select.width();
            if (options.width) {
                width = options.width;
            }
            if (($option.height() + top) < $(window).height()) {

                $option.slideDown(150).css({ top: top, left: left, width: width });
            } else {
                var _top = (top - $option.height() - 32);
                $option.show().css({ top: _top, left: left, width: width });
                $option.attr('data-show', true);
            }
            $option.css('border-top', '1px solid #ccc');
            if (options.appendTo) {
                $option.css("position", "inherit")
            }
            $option.find('.ui-select-option-search').find('input').select();
        } else {
            if ($option.attr('data-show')) {
                $option.hide();
            } else {
                $option.slideUp(150);
            }
        }
        e.stopPropagation();
    });
    $select.find('li div').click(function (e) {
        var e = e ? e : window.event;
        var tar = e.srcElement || e.target;
        if (!$(tar).hasClass('bbit-tree-ec-icon')) {
            $option.slideUp(150);
            e.stopPropagation();
        }
    });
    $(document).click(function (e) {
        var e = e ? e : window.event;
        var tar = e.srcElement || e.target;
        if (!$(tar).hasClass('bbit-tree-ec-icon') && !$(tar).hasClass('form-control')) {
            if ($option.attr('data-show')) {
                $option.hide();
            } else {
                $option.slideUp(150);
            }
            $select.removeClass('ui-select-focus');
            e.stopPropagation();
        }
    });
    return $select;
}
$.fn.ComboBoxTreeSetValue = function (value) {
    if (value == "") {
        return;
    }
    var $select = $(this);
    var $option = $("#" + $select.attr('id') + "-option");
    $select.attr('data-value', value);
    var $item = $option.find('ul').find('[data-value=' + value + ']');
    var data_text = $item.html();
    var data_code = $item.attr("data-code");
    var data_dept = $item.attr("data-dept");
    if (data_code) {
        $select.attr('data-code', data_code);
    }
    if (data_dept) {
        $select.attr('data-dept', data_dept);
    }
    if (data_text) {
        $select.attr('data-text', data_text);
        $select.find('.ui-select-text').html(data_text).css('color', '#000');
        $option.find('ul').find('[data-value=' + value + ']').parent().parent().addClass('bbit-tree-selected');
    }
    return $select;
}
/**扩展方法  获取ComboBoxTree Value值*/
$.fn.ComboBoxTreeGetValue = function () {
    var $select = $(this);
    var $option = $("#" + $select.attr('id') + "-option");
    var data_text = $select.attr('data-value'); //获取text值
    return data_text;
}
/**扩展方法 设置ComboBoxTree Text值
 *@param {string} text 要设置的文字
 * */
$.fn.ComboBoxTreeSetText = function (text) {

    if ($.isNullOrEmpty(text)) {
        return;
    }
    var $select = $(this);
    var $option = $("#" + $select.attr('id') + "-option");
    $select.attr('data-text', text);
    var value = "";  //值
    $option.find('ul li span').each(function (index, ele) {
        if ($(this).html() == text) {
            value = $(this).attr('data-value');
        }
    });
    if (!!text) {
        $select.attr('data-value', value);
        $select.find('.ui-select-text').html(text).css('color', '#000');
        $option.find('ul').find("[data-value='" + value + "']").parent().parent().addClass('bbit-tree-selected');
    }
    return $select;
}
/**扩展方法  获取ComboBox Text值*/
$.fn.ComboBoxTreeGetText = function () {
    var $select = $(this);
    var $option = $("#" + $select.attr('id') + "-option");
    var data_text = $select.attr('data-text'); //获取text值
    return data_text;
}
$.fn.formSerialize = function (keyValue) {
    var reVal = "";
    $(this).find('input,select,textarea,.ui-select').each(function (r) {
        var id = $(this).attr('id');
        var type = $(this).attr('type');
        switch (type) {
            case "checkbox":
                if ($("#" + id).is(":checked")) {
                    reVal += '"' + id + '"' + ':' + '"1",'
                } else {
                    reVal += '"' + id + '"' + ':' + '"0",'
                }
                break;
            case "select":
                var value = $("#" + id).attr('data-value');
                if (value == undefined) {
                    value = $("#" + id).val();
                }
                else {
                    //if (value == "") {
                    //    value = "&nbsp;";
                    //}
                }
                reVal += '"' + id + '"' + ':' + '"' + $.trim(value) + '",'
                break;
            case "selectTree":
                var value = $("#" + id).attr('data-value');
                //if (value == "") {
                //    value = "&nbsp;";
                //}
                reVal += '"' + id + '"' + ':' + '"' + $.trim(value) + '",'
                break;
            default:
                var value = $("#" + id).val();
                //if (value == "") {
                //    value = "&nbsp;";
                //}
                reVal += '"' + id + '"' + ':' + '"' + $.trim(value) + '",'
                break;
        }
    });
    reVal = reVal.substr(0, reVal.length - 1);
    if (!keyValue) {
        reVal = reVal.replace(/&nbsp;/g, '');
    }
    reVal = reVal.replace(/\\/g, '\\\\');
    reVal = reVal.replace(/\n/g, '\\n');
    var postdata = jQuery.parseJSON('{' + reVal + '}');
    //阻止伪造请求
    //if ($('[name=__RequestVerificationToken]').length > 0) {
    //    postdata["__RequestVerificationToken"] = $('[name=__RequestVerificationToken]').val();
    //}
    return postdata;
};
$.fn.formDeserialize = function (data) {
    var $id = $(this);
    for (var key in data) {
        var id = $id.find('#' + key);
        if (id.attr('id')) {
            var type = id.attr('type');
            if (id.hasClass("input-datepicker")) {
                type = "datepicker";
            }
            var value = $.trim(data[key]).replace(/&nbsp;/g, '');
            switch (type) {
                case "checkbox":
                    if (value == 1) {
                        id.attr("checked", 'checked');
                    } else {
                        id.removeAttr("checked");
                    }
                    break;
                case "select":
                    id.ComboBoxSetValue(value);
                    break;
                case "selectTree":
                    id.ComboBoxTreeSetValue(value);
                    break;
                case "datepicker":
                    id.val(formatDate(value, 'yyyy-MM-dd'));
                    break;
                default:
                    if (id.hasClass("input-wdatepicker")) {
                        id.val(value.replace("00:00:00", ""));
                    } else {
                        if (value.indexOf("00:00:00") >= 0) {
                            id.val(value.replace("00:00:00", ""));
                        } else {
                            id.val(value);
                        }
                    }
                    break;
            }
        }
    }
}
$.fn.Contextmenu = function () {
    var element = $(this);
    var oMenu = $('.contextmenu');
    $(document).click(function () {
        oMenu.hide();
    });
    $(document).mousedown(function (e) {
        if (3 == e.which) {
            oMenu.hide();
        }
    })
    var aUl = oMenu.find("ul");
    var aLi = oMenu.find("li");
    var showTimer = hideTimer = null;
    var i = 0;
    var maxWidth = maxHeight = 0;
    var aDoc = [document.documentElement.offsetWidth, document.documentElement.offsetHeight];
    oMenu.hide();
    for (i = 0; i < aLi.length; i++) {
        //为含有子菜单的li加上箭头
        aLi[i].getElementsByTagName("ul")[0] && (aLi[i].className = "sub");
        //鼠标移入
        aLi[i].onmouseover = function () {
            var oThis = this;
            var oUl = oThis.getElementsByTagName("ul");
            //鼠标移入样式
            oThis.className += " active";
            //显示子菜单
            if (oUl[0]) {
                clearTimeout(hideTimer);
                showTimer = setTimeout(function () {
                    for (i = 0; i < oThis.parentNode.children.length; i++) {
                        oThis.parentNode.children[i].getElementsByTagName("ul")[0] &&
                            (oThis.parentNode.children[i].getElementsByTagName("ul")[0].style.display = "none");
                    }
                    oUl[0].style.display = "block";
                    oUl[0].style.top = oThis.offsetTop + "px";
                    oUl[0].style.left = oThis.offsetWidth + "px";

                    //最大显示范围					
                    maxWidth = aDoc[0] - oUl[0].offsetWidth;
                    maxHeight = aDoc[1] - oUl[0].offsetHeight;

                    //防止溢出
                    maxWidth < getOffset.left(oUl[0]) && (oUl[0].style.left = -oUl[0].clientWidth + "px");
                    maxHeight < getOffset.top(oUl[0]) && (oUl[0].style.top = -oUl[0].clientHeight + oThis.offsetTop + oThis.clientHeight + "px")
                }, 300);
            }
        };
        //鼠标移出	
        aLi[i].onmouseout = function () {
            var oThis = this;
            var oUl = oThis.getElementsByTagName("ul");
            //鼠标移出样式
            oThis.className = oThis.className.replace(/\s?active/, "");

            clearTimeout(showTimer);
            hideTimer = setTimeout(function () {
                for (i = 0; i < oThis.parentNode.children.length; i++) {
                    oThis.parentNode.children[i].getElementsByTagName("ul")[0] &&
                        (oThis.parentNode.children[i].getElementsByTagName("ul")[0].style.display = "none");
                }
            }, 300);
        };
    }
    //自定义右键菜单
    $(element).bind("contextmenu", function () {
        var event = event || window.event;
        oMenu.show();
        oMenu.css('top', event.clientY + "px");
        oMenu.css('left', event.clientX + "px");
        //最大显示范围
        maxWidth = aDoc[0] - oMenu.width();
        maxHeight = aDoc[1] - oMenu.height();
        //防止菜单溢出
        if (oMenu.offset().top > maxHeight) {
            oMenu.css('top', maxHeight + "px");
        }
        if (oMenu.offset().left > maxWidth) {
            oMenu.css('left', maxWidth + "px");
        }
        return false;
    }).bind("click", function () {
        oMenu.hide();
    });
}
$.fn.panginationEx = function (options) {
    var $pager = $(this);
    if (!$pager.attr('id')) {
        return false;
    }
    var defaults = {
        firstBtnText: '首页',
        lastBtnText: '尾页',
        prevBtnText: '上一页',
        nextBtnText: '下一页',
        showInfo: true,
        showJump: true,
        jumpBtnText: '跳转',
        showPageSizes: true,
        infoFormat: '{start} ~ {end}条，共{total}条',
        sortname: '',
        url: "",
        success: null,
        beforeSend: null,
        complete: null
    };
    var options = $.extend(defaults, options);
    var params = $.extend({ sidx: options.sortname, sord: "asc" }, options.params);
    options.remote = {
        url: options.url,  //请求地址
        params: params,       //自定义请求参数
        beforeSend: function (XMLHttpRequest) {
            if (options.beforeSend != null) {
                options.beforeSend(XMLHttpRequest);
            }
        },
        success: function (result, pageIndex) {
            //回调函数
            //result 为 请求返回的数据，呈现数据
            if (options.success != null) {
                options.success(result.rows, pageIndex);
            }
        },
        complete: function (XMLHttpRequest, textStatu) {
            if (options.complete != null) {
                options.complete(XMLHttpRequest, textStatu);
            }
            //...
        },
        pageIndexName: 'page',     //请求参数，当前页数，索引从0开始
        pageSizeName: 'rows',       //请求参数，每页数量
        totalName: 'records'              //指定返回数据的总数据量的字段名
    }
    $pager.page(options);
}
$.fn.LeftListShowOfemail = function (options) {
    var $list = $(this);
    if (!$list.attr('id')) {
        return false;
    }
    $list.append('<ul  style="padding-top: 10px;"></ul>');
    var defaults = {
        id: "id",
        name: "text",
        img: "fa fa-file-o"
    };
    var options = $.extend(defaults, options);
    $list.height(options.height);
    $.ajax({
        url: options.url,
        data: options.param,
        type: "GET",
        dataType: "json",
        async: false,
        success: function (data) {
            $.each(data, function (i, item) {
                var $_li = $('<li class="" data-value="' + item[options.id] + '"  data-text="' + item[options.name] + '" ><i class="' + options.img + '" style="vertical-align: middle; margin-top: -2px; margin-right: 8px; font-size: 14px; color: #666666; opacity: 0.9;"></i>' + item[options.name] + '</li>');
                if (i == 0) {
                    $_li.addClass("active");
                }
                $list.find('ul').append($_li);
            });
            $list.find('li').click(function () {
                var key = $(this).attr('data-value');
                var value = $(this).attr('data-text');
                $list.find('li').removeClass('active');
                $(this).addClass('active');
                options.onnodeclick({ id: key, name: value });
            });
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            dialogMsg(errorThrown, -1);
        }
    });
}
$.fn.authorizeButton = function (args) {
    var $element = $(this);
    //$element.find('a.btn').attr('authorize', 'no')
    //$element.find('ul.dropdown-menu').find('li').attr('authorize', 'no')
    //var moduleId = tabiframeId().substr(6);
    //var data = top.authorizeButtonData[moduleId];
    //if (data != undefined) {
    //    $.each(data, function (i) {
    //        var id = "lr-"+data[i].EnCode;
    //        $element.find("#" + id).attr('authorize', 'yes');
    //    });
    //}
    //$element.find('[authorize=no]').remove();
    var enCode = "";
    if (args == undefined || args == null) {
        $element.find('a.btn').each(function (i, dom) {
            enCode += dom.id.replace("lr-", "") + ",";
        });
        $element.find('ul.dropdown-menu').find('li').each(function (i, dom) {
            enCode += dom.id.replace("lr-", "") + ",";
        });

    } else {
        enCode = args;
    }
    console.log(enCode);
    //var path = document.baseURI;
    //path = path.replace(top.location.origin, "");
    //var ind = path.indexOf('/', 2);
    //path = path.substring(0, ind);
    $.post(top.contentPath  + "/AuthorizeManage/PermissionJob/GetOperAuthority", { enCode: enCode }, function (data) {
        var html = "";
        //if (enCode.length == 0) {
        //    if (data.length > 0) {
        //        var json = eval("(" + data + ")");
        //        $(json).each(function (i, item) {
        //            html += '<a id="' + item.encode + '" class=\"btn btn-default\"';
        //            if (item.actionname != undefined && item.actionname != null) {
        //                html += ' onclick="' + item.actionname + '()"';
        //            }
        //            html += '>';
        //            if (item.faimage != undefined && item.faimage != null) {
        //                if (item.faimage.indexOf("fa") >= 0) {
        //                    html += ' <i class="' + item.faimage + '"></i>';
        //                }

        //            }
        //            html += ' &nbsp;' + item.fullname + '</a>';
        //        });
        //        $(".toolbar").find(".btn-group").html(html);
        //    }
        //} else {
        if (data.length == 0) {
            //兼容处理，如果未配置权限，则默认为页面自定义按钮
            //$('.toolbar').find("a[id]").remove();
        } else {
            var json = eval("(" + data + ")");
            console.log(data);
            $(json).each(function (i, item) {
                enCode = enCode.replace(item.encode + ",", "");
                if ($element.find("#" + item.encode).length == 0 && $element.find("#lr-" + item.encode).length == 0) {
                    html = '<a id="' + item.encode + '" class=\"btn btn-default\"';
                    if (item.actionname != undefined && item.actionname != null) {
                        html += ' onclick="' + item.actionname + '()"';
                    }
                    html += '>';
                    if (item.faimage != undefined && item.faimage != null) {
                        if (item.faimage.length > 0) {
                            html += ' <i class="' + item.faimage + '"></i>';
                        }

                    }
                    html += ' &nbsp;' + item.fullname + '</a>';
                    $element.append(html);
                }
            });
            if (enCode.length > 0) {
                var arr = enCode.split(',');
                for (var j = 0; j < arr.length; j++) {
                    if (arr[j] != "more" && arr[j].length > 0) {
                        var id = arr[j];
                        $element.find("#" + id).remove();
                    }

                }
            }
        }
        //}
    });
}
//按钮权限 --何明
function AuthiruzeBtn(toolbar, grid) {
    console.log(top.contentPath  + "/AuthorizeManage/PermissionJob/GetAuthorityBotton")
    $.post(path+ "/AuthorizeManage/PermissionJob/GetAuthorityBotton", function (data) {
        if (data != "") {
            //移除toolbar里面的未授权按钮
            if (data.operScope.length > 0) {
                var operScope = eval("(" + data.operScope + ")");
                $(toolbar).find("a[code]").each(function (i, elemt) {
                    var isHave = false;
                    $(operScope).each(function (i, item) {
                        if ($(elemt).attr("id") == item.encode && !isHave) {
                            isHave = true
                        }
                    });
                    if (!isHave) {
                        $(elemt).remove();
                    }
                })
            } else {
                //移除toolbar里面的未授权按钮
                $(toolbar).find("a[code]").each(function (i, elemt) {
                    $(elemt).remove();
                })
            }

            //移除列表里的不符合规则的按钮
            if (data.dataScope.length > 0) {
                var dataScope = eval("(" + data.dataScope + ")");
                $(grid).find("a[code]").each(function (i, btn) {
                    var isHave = false;
                    $(dataScope).each(function (i, code) {
                        if ($(btn).attr("code") == code.encode && !isHave) {
                            isHave = true
                        }
                    })
                    if (!isHave) {
                        $(btn).remove();
                    }
                });
            } else {
                //移除列表里的不符合规则的按钮
                $(grid).find("a[code]").each(function (i, btn) {
                    $(btn).remove();
                })
            }
        } else {
            //移除toolbar里面的未授权按钮
            $(toolbar).find("a[code]").each(function (i, elemt) {
                $(elemt).remove();
            })
            //移除列表里的不符合规则的按钮
            $(grid).find("a[code]").each(function (i, btn) {
                $(btn).remove();
            })
        }
    });
}

$.fn.authorizeColModel = function () {
    var $element = $(this);
    var columnModel = $element.jqGrid('getGridParam', 'colModel');
    $.each(columnModel, function (i) {
        if (columnModel[i].name != "rn") {
            $element.hideCol(columnModel[i].name);
        }
    });
    var moduleId = tabiframeId().substr(6);
    var data = top.authorizeColumnData[moduleId];
    if (data != undefined) {
        $.each(data, function (i) {
            $element.showCol(data[i].EnCode);
        });
    }
}
$.fn.jqGridEx = function (options) {
    var $jqGrid = $(this);
    var _selectedRowIndex;
    if (!$jqGrid.attr('id')) {
        return false;
    }
    var isAuth = options.isAuth;
    var defaults = {
        url: "",
        datatype: "json",
        height: $(window).height() - 139.5,
        autowidth: true,
        colModel: [],
        viewrecords: true,
        rowNum: 30,
        rowList: [30, 50, 100],
        pager: "#gridPager",
        rownumbers: true,
        shrinkToFit: false,
        gridview: true,
        gridComplete: function () {
            if (isAuth || isAuth == undefined) {
                var rows = $jqGrid.jqGrid("getRowData");//获取当前页记录行数据
                if (rows.length > 0) {
                    var row = rows[0];
                    // if (row.createuserid != undefined && row.createuserdeptcode != undefined && row.createuserorgcode != undefined) {
                    //查询用户对该模块的数据操作权限
                    $.post(top.contentPath + "/AuthorizeManage/PermissionJob/GetDataAuthority", { __RequestVerificationToken: $("input[name='__RequestVerificationToken']").val() }, function (data) {
                        var jsonArr = eval("(" + data + ")");

                        $(rows).each(function (i, item) {
                            var uId = item.createuserid;//获取记录创建人的Id
                            var dCode = item.createuserdeptcode; //获取记录创建人的所属部门Code
                            var oCode = item.createuserorgcode;  //获取记录创建人的所属机构Code
                            var btns = $("td[aria-describedby='gridTable_Oper']").eq(i);//获取操作列中定义的操作按钮
                            var html = "";
                            //如果操作列中没有定义任何按钮则根据系统权限设置自动绑定操作按钮
                            //if (btns.length == 0) {
                            $(jsonArr).each(function (j, item1) {
                                var authType = parseInt(item1.authorizetype);//获取数据操作权限范围.1：本人,2：本部门，3：本部门及下属部门，4：本机构，5：全部
                                switch (authType) {
                                    //本用户
                                    case 1:
                                        if (top.currUserId == uId) {
                                            if (btns.find("a[name='" + item1.encode + "']").length == 0) {
                                                html += "<a href=\"javascript:" + item1.actionname + "()\" title=\"" + item1.fullname + "\"><i class=\"" + item1.faimage + "\"></i></a>";
                                            }
                                        } else {
                                            if (btns.find("a[name='" + item1.encode + "']").length > 0) {
                                                btns.find("a[name='" + item1.encode + "']").remove();
                                            }
                                        }
                                        break;
                                    //本部门
                                    case 2:
                                        if (top.currUserDeptCode == dCode) {
                                            if (btns.find("a[name='" + item1.encode + "']").length == 0) {
                                                html += "<a href=\"javascript:" + item1.actionname + "()\" title=\"" + item1.fullname + "\"><i class=\"" + item1.faimage + "\"></i></a>";
                                            }
                                        } else {
                                            if (btns.find("a[name='" + item1.encode + "']").length > 0) {
                                                btns.find("a[name='" + item1.encode + "']").remove();
                                            }
                                        }
                                        break;
                                    //本子部门
                                    case 3:
                                        if (dCode.indexOf(top.currUserDeptCode) >= 0) {
                                            if (btns.find("a[name='" + item1.encode + "']").length == 0) {
                                                html += "<a href=\"javascript:" + item1.actionname + "()\" title=\"" + item1.fullname + "\"><i class=\"" + item1.faimage + "\"></i></a>";
                                            }
                                        } else {
                                            if (btns.find("a[name='" + item1.encode + "']").length > 0) {
                                                btns.find("a[name='" + item1.encode + "']").remove();
                                            }
                                        }
                                        break;
                                    //本机构
                                    case 4:
                                        if (oCode == top.currUserOrgCode) {
                                            if (btns.find("a[name='" + item1.encode + "']").length == 0) {
                                                html += "<a href=\"javascript:" + item1.actionname + "()\" title=\"" + item1.fullname + "\"><i class=\"" + item1.faimage + "\"></i></a>";
                                            }
                                        } else {
                                            if (btns.find("a[name='" + item1.encode + "']").length > 0) {
                                                btns.find("a[name='" + item1.encode + "']").remove();
                                            }
                                        }
                                        break;
                                    case 5:
                                        if (btns.find("a[name='" + item1.encode + "']").length == 0) {
                                            html += "<a href=\"javascript:" + item1.actionname + "()\" title=\"" + item1.fullname + "\"><i class=\"" + item1.faimage + "\"></i></a>";
                                        }
                                        //html += "<a href=\"javascript:" + item1.actionname + "()\" title=\"" + item1.fullname + "\"><i class=\"" + item1.faimage + "\"></i></a>";
                                        break;
                                }
                            });
                            $("td[aria-describedby='gridTable_Oper']").eq(i).append(html);
                        });
                    });
                    // }
                }
            }
        }
    };
    var options = $.extend(defaults, options);
    $jqGrid.jqGrid(options);
}
$.fn.jqGridRowValue = function (code) {
    var $jgrid = $(this);
    var json = [];
    var selectedRowIds = $jgrid.jqGrid("getGridParam", "selarrrow");
    if (selectedRowIds != undefined && selectedRowIds != "") {
        var len = selectedRowIds.length;
        for (var i = 0; i < len; i++) {
            var rowData = $jgrid.jqGrid('getRowData', selectedRowIds[i]);
            json.push(rowData[code]);
        }
    } else {
        var rowData = $jgrid.jqGrid('getRowData', $jgrid.jqGrid('getGridParam', 'selrow'));
        json.push(rowData[code]);
    }
    return String(json);
}
$.fn.jqGridRow = function () {
    var $jgrid = $(this);
    var json = [];
    var selectedRowIds = $jgrid.jqGrid("getGridParam", "selarrrow");
    if (selectedRowIds != "") {
        var len = selectedRowIds.length;
        for (var i = 0; i < len; i++) {
            var rowData = $jgrid.jqGrid('getRowData', selectedRowIds[i]);
            json.push(rowData);
        }
    } else {
        var rowData = $jgrid.jqGrid('getRowData', $jgrid.jqGrid('getGridParam', 'selrow'));
        json.push(rowData);
    }
    return json;
}
dialogTop = function (content, type) {
    $(".tip_container").remove();
    var bid = parseInt(Math.random() * 100000);
    $("body").prepend('<div id="tip_container' + bid + '" class="container tip_container"><div id="tip' + bid + '" class="mtip"><i class="micon"></i><span id="tsc' + bid + '"></span><i id="mclose' + bid + '" class="mclose"></i></div></div>');
    var $this = $(this);
    var $tip_container = $("#tip_container" + bid);
    var $tip = $("#tip" + bid);
    var $tipSpan = $("#tsc" + bid);
    //先清楚定时器
    clearTimeout(window.timer);
    //主体元素绑定事件
    $tip.attr("class", type).addClass("mtip");
    $tipSpan.html(content);
    $tip_container.slideDown(300);
    //提示层隐藏定时器
    window.timer = setTimeout(function () {
        $tip_container.slideUp(300);
        $(".tip_container").remove();
    }, 4000);
    $("#tip_container" + bid).css("left", ($(window).width() - $("#tip_container" + bid).width()) / 2);
}
dialogOpen = function (options) {
    Loading(true);
    var defaults = {
        id: null,
        title: '系统窗口',
        width: "100px",
        height: "100px",
        url: '',
        shade: 0.3,
        btn: ['确认', '关闭'],
        callBack: null
    };
    var options = $.extend(defaults, options);
    var _url = options.url;
    var _width = top.$.windowWidth() > parseInt(options.width.replace('px', '')) ? options.width : top.$.windowWidth() + 'px';
    var _height = top.$.windowHeight() > parseInt(options.height.replace('px', '')) ? options.height : top.$.windowHeight() + 'px';
    return top.layer.open({
        id: options.id,
        type: 2,
        shade: options.shade,
        title: options.title,
        fix: false,
        area: [_width, _height],
        content: top.contentPath + _url,
        btn: options.btn,
        yes: function () {
            options.callBack(options.id)
        }, cancel: function () {
            if (options.cancel != undefined) {
                options.cancel();
            }
            return true;
        }, end: options.end
    });
}
dialogOpen2 = function (options) {
    Loading(true);
    var defaults = {
        id: null,
        title: '系统窗口',
        width: "100px",
        height: "100px",
        url: '',
        shade: 0.3,
        btn: ['确认', '关闭'],
        callBack: null
    };
    var options = $.extend(defaults, options);
    var _url = options.url;
    var _width = top.$.windowWidth() > parseInt(options.width.replace('px', '')) ? options.width : top.$.windowWidth() + 'px';
    var _height = top.$.windowHeight() > parseInt(options.height.replace('px', '')) ? options.height : top.$.windowHeight() + 'px';
    return top.layer.open({
        id: options.id,
        type: 2,
        shade: options.shade,
        title: options.title,
        fix: false,
        area: [_width, _height],
        content: _url,
        btn: options.btn,
        yes: function () {
            options.callBack(options.id)
        }, cancel: function () {
            if (options.cancel != undefined) {
                options.cancel();
            }
            return true;
        }, end: options.end
    });
}
dialogOpenFrame = function (options) {
    Loading(true);
    var defaults = {
        id: null,
        title: '系统窗口',
        width: "100px",
        height: "100px",
        url: '',
        shade: 0.3,
        btn: ['关闭'],
        callBack: null
    };
    var options = $.extend(defaults, options);
    var _url = options.url;
    var _width = top.$.windowWidth() > parseInt(options.width.replace('px', '')) ? options.width : top.$.windowWidth() + 'px';
    var _height = top.$.windowHeight() > parseInt(options.height.replace('px', '')) ? options.height : top.$.windowHeight() + 'px';
    top.layer.open({
        id: options.id,
        type: 2,
        shade: options.shade,
        title: options.title,
        fix: false,
        area: [_width, _height],
        content: top.contentPath + _url,
        btn: options.btn,
        cancel: function () {
            if (options.cancel != undefined) {
                options.cancel();
            }
            return true;
        }, end: options.end
    });
}
dialogContent = function (options) {
    var defaults = {
        id: null,
        title: '系统窗口',
        width: "100px",
        height: "100px",
        content: '',
        btn: ['确认', '关闭'],
        callBack: null
    };
    var options = $.extend(defaults, options);
    return top.layer.open({
        id: options.id,
        type: 1,
        title: options.title,
        fix: false,
        area: [options.width, options.height],
        content: options.content,
        btn: options.btn,
        yes: function () {
            options.callBack(options.id)
        }
    });
}
dialogAlert = function (content, type) {
    if (type == -1) {
        type = 2;
    }
    return top.layer.alert(content, {
        icon: type,
        title: "系统提示"
    });
}
dialogConfirm = function (content, callBack) {
    return top.layer.confirm(content, {
        icon: 7,
        title: "系统提示",
        btn: ['确认', '取消']
    }, function () {
        callBack(true);
    }, function () {
        callBack(false)
    });
}
dialogMsg = function (content, type) {
    if (type == -1) {
        type = 2;
    }
    return top.layer.msg(content, { icon: type, time: 2000, shift: 5 });
}
dialogClose = function () {
    try {
        var index = top.layer.getFrameIndex(window.name); //先得到当前iframe层的索引
        var $IsdialogClose = top.$("#layui-layer" + index).find('.layui-layer-btn').find("#IsdialogClose");
        var IsClose = $IsdialogClose.is(":checked");
        if ($IsdialogClose.length == 0) {
            IsClose = true;
        }
        if (IsClose) {
            top.layer.close(index);
        } else {
            location.reload();
        }
    } catch (e) {
        alert(e)
    }
}
reload = function () {
    location.reload();
    return false;
}
newGuid = function () {
    var guid = "";
    for (var i = 1; i <= 32; i++) {
        var n = Math.floor(Math.random() * 16.0).toString(16);
        guid += n;
        if ((i == 8) || (i == 12) || (i == 16) || (i == 20)) guid += "-";
    }
    return guid;
}
formatDate = function (v, format) {
    if (!v) return "";
    var d = v;
    if (typeof v === 'string') {
        if (v.indexOf("/Date(") > -1)
            d = new Date(parseInt(v.replace("/Date(", "").replace(")/", ""), 10));
        else
            d = new Date(Date.parse(v.replace(/-/g, "/").replace("T", " ").split(".")[0]));//.split(".")[0] 用来处理出现毫秒的情况，截取掉.xxx，否则会出错
    }
    var o = {
        "M+": d.getMonth() + 1,  //month
        "d+": d.getDate(),       //day
        "h+": d.getHours(),      //hour
        "m+": d.getMinutes(),    //minute
        "s+": d.getSeconds(),    //second
        "q+": Math.floor((d.getMonth() + 3) / 3),  //quarter
        "S": d.getMilliseconds() //millisecond
    };
    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (d.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
};
toDecimal = function (num) {
    if (num == null) {
        num = "0";
    }
    num = num.toString().replace(/\$|\,/g, '');
    if (isNaN(num))
        num = "0";
    sign = (num == (num = Math.abs(num)));
    num = Math.floor(num * 100 + 0.50000000001);
    cents = num % 100;
    num = Math.floor(num / 100).toString();
    if (cents < 10)
        cents = "0" + cents;
    for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++)
        num = num.substring(0, num.length - (4 * i + 3)) + '' +
            num.substring(num.length - (4 * i + 3));
    return (((sign) ? '' : '-') + num + '.' + cents);
}
Date.prototype.DateAdd = function (strInterval, Number) {
    //y年 q季度 m月 d日 w周 h小时 n分钟 s秒 ms毫秒
    var dtTmp = this;
    switch (strInterval) {
        case 's': return new Date(Date.parse(dtTmp) + (1000 * Number));
        case 'n': return new Date(Date.parse(dtTmp) + (60000 * Number));
        case 'h': return new Date(Date.parse(dtTmp) + (3600000 * Number));
        case 'd': return new Date(Date.parse(dtTmp) + (86400000 * Number));
        case 'w': return new Date(Date.parse(dtTmp) + ((86400000 * 7) * Number));
        case 'q': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number * 3, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
        case 'm': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
        case 'y': return new Date((dtTmp.getFullYear() + Number), dtTmp.getMonth(), dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
    }
}
request = function (keyValue) {
    var search = location.search.slice(1);
    var arr = search.split("&");
    for (var i = 0; i < arr.length; i++) {
        var ar = arr[i].split("=");
        if (ar[0] == keyValue) {
            if (decodeURIComponent(ar[1]) == 'undefined') {
                return "";
            } else {
                return decodeURIComponent(ar[1]);
            }
        }
    }
    return "";
}
changeUrlParam = function (url, key, value) {
    var newUrl = "";
    var reg = new RegExp("(^|)" + key + "=([^&]*)(|$)");
    var tmp = key + "=" + value;
    if (url.match(reg) != null) {
        newUrl = url.replace(eval(reg), tmp);
    } else {
        if (url.match("[\?]")) {
            newUrl = url + "&" + tmp;
        }
        else {
            newUrl = url + "?" + tmp;
        }
    }
    return newUrl;
}
$.currentIframe = function () {

    if ($.isbrowsername() == "IE") {
        return top.frames[tabiframeId()];
    }
    else {
        return top.frames[tabiframeId()];
        //return top.window.document.getElementById(tabiframeId()).contentWindow;
    }
}
$.isbrowsername = function () {
    var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串
    var isOpera = userAgent.indexOf("Opera") > -1;
    if (isOpera) {
        return "Opera"
    }; //判断是否Opera浏览器
    if (userAgent.indexOf("Firefox") > -1) {
        return "FF";
    } //判断是否Firefox浏览器
    if (userAgent.indexOf("Chrome") > -1) {
        //if (window.navigator.webkitPersistentStorage.toString().indexOf('DeprecatedStorageQuota') > -1) {
        return "Chrome";
        //} else {
        //    return "360";
        //}
    }//判断是否Chrome浏览器//360浏览器
    if (userAgent.indexOf("Safari") > -1) {
        return "Safari";
    } //判断是否Safari浏览器
    if (userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1 && !isOpera) {
        return "IE";
    }; //判断是否IE浏览器
}
$.download = function (url, data, method) {
    if (url && data) {
        data = typeof data == 'string' ? data : jQuery.param(data);
        var inputs = '';
        $.each(data.split('&'), function () {
            var pair = this.split('=');
            inputs += '<input type="hidden" name="' + pair[0] + '" value="' + pair[1] + '" />';
        });
        $('<form action="' + url + '" method="' + (method || 'post') + '">' + inputs + '</form>').appendTo('body').submit().remove();
    };
};
$.standTabchange = function (object, forid) {
    $(".standtabactived").removeClass("standtabactived");
    $(object).addClass("standtabactived");
    $('.standtab-pane').css('display', 'none');
    $('#' + forid).css('display', 'block');
}
$.isNullOrEmpty = function (obj) {
    if ((typeof (obj) == "string" && obj == "") || obj == null || obj == undefined) {
        return true;
    }
    else {
        return false;
    }
}
$.arrayClone = function (data) {
    return $.map(data, function (obj) {
        return $.extend(true, {}, obj);
    });
}
$.windowWidth = function () {
    return $(window).width();
}
$.windowHeight = function () {
    return $(window).height();
}
IsNumber = function (obj) {
    $("#" + obj).bind("contextmenu", function () {
        return false;
    });
    $("#" + obj).css('ime-mode', 'disabled');
    $("#" + obj).keypress(function (e) {
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            return false;
        }
    });
}
IsMoney = function (obj) {
    $("#" + obj).bind("contextmenu", function () {
        return false;
    });
    $("#" + obj).css('ime-mode', 'disabled');
    $("#" + obj).bind("keydown", function (e) {
        var key = window.event ? e.keyCode : e.which;
        if (isFullStop(key)) {
            return $(this).val().indexOf('.') < 0;
        }
        return (isSpecialKey(key)) || ((isNumber(key) && !e.shiftKey));
    });
    function isNumber(key) {
        return key >= 48 && key <= 57
    }
    function isSpecialKey(key) {
        return key == 8 || key == 46 || (key >= 37 && key <= 40) || key == 35 || key == 36 || key == 9 || key == 13
    }
    function isFullStop(key) {
        return key == 190 || key == 110;
    }
}
checkedArray = function (id) {
    var isOK = true;
    if (id == undefined || id == "" || id == 'null' || id == 'undefined') {
        isOK = false;
        dialogMsg('您没有选中任何项,请您选中后再操作。', 0);
    }
    return isOK;
}
checkedRow = function (id) {
    var isOK = true;
    if (id == undefined || id == "" || id == 'null' || id == 'undefined') {
        isOK = false;
        dialogMsg('您没有选中任何数据项,请选中后再操作！', 0);
    } else if (id.split(",").length > 1) {
        isOK = false;
        dialogMsg('很抱歉,一次只能选择一条记录！', 0);
    }
    return isOK;
};
$(function () {
    $(".ui-filter-text").click(function () {
        if ($(this).next('.ui-filter-list').is(":hidden")) {
            $(this).css('border-bottom-color', '#fff');
            $(".ui-filter-list").slideDown(10);
            $(this).addClass("active")
        } else {
            $(this).css('border-bottom-color', '#ccc');
            $(".ui-filter-list").slideUp(10);
            $(this).removeClass("active")
        }
    });
    $(".profile-nav li").click(function () {
        $(".profile-nav li").removeClass("active");
        $(".profile-nav li").removeClass("hover");
        $(this).addClass("active")
    }).hover(function () {
        if (!$(this).hasClass("active")) {
            $(this).addClass("hover")
        }
    }, function () {
        $(this).removeClass("hover")
    });
    jQuery('.panel .tools .fa-chevron-down').click(function () {
        var el = jQuery(this).parents(".panel").children(".panel-body");
        if (jQuery(this).hasClass("fa-chevron-down")) {
            jQuery(this).removeClass("fa-chevron-down").addClass("fa-chevron-up");
            el.slideUp(200);
        } else {
            jQuery(this).removeClass("fa-chevron-up").addClass("fa-chevron-down");
            el.slideDown(200);
        }
    });
});
/**
 * 菜单处理
 * @param {Object} menuli 第一级菜单所在的元素(Jquery对象)，遍历会遍历这个元素底下的li
 * @param {Object} childElemt 第二级菜单所在的元素(Jquery对象)，遍历会遍历这个元素底下的div
 */
function InitMenu(menuli, childElemt) {
    console.log(document.baseURI);
    var locaPath = top.contentPath ;//本地调用 
    //在双控下调用把locaPath改为path
    $.post(locaPath + "/Home/GetMenu", function (data) {
        if (data.Code == 0) {
            var menuList = data.data;

            if (!!menuli) {
                $(menuli).find("li[code]").each(function (i, elemt) {
                    var isHave = false;
                    $(menuList).each(function (i, item) {
                        if ($(elemt).attr("code") == item.ModuleCode && !isHave) {
                            console.log($(elemt).attr("code") + " | " + item.ModuleCode + "|" + true)
                            isHave = true
                            return false;//已找到，结束此循环
                        }
                    });
              
                    if (isHave) {
                        //有的才显示
                        $(elemt).show();
                    } else {
                        //没有的则隐藏
                        $(elemt).hide();
                    }
                })
            }

            if (!!childElemt) {
                //div
                $(childElemt).find("div[code]").each(function (i, divelemt) {
                    var isHave = false;
                    $(menuList).each(function (i, item) {
                        if ($(divelemt).attr("code") == item.ModuleCode && !isHave) {
                            isHave = true
                            return false;//已找到，结束此循环
                        }
                    });
                    if (isHave) {
                        //有的才显示
                        $(divelemt).show();
                    } else {
                        //没有的则隐藏
                        $(divelemt).hide();
                    }
                })
            }
            
        } else {
            //没找到菜单隐藏所有
            if (!!menuli) {
                $(menuli).find("li[code]").each(function (i, elemt) {
                    $(elemt).hide();
                })
            }
           
            if (!!childElemt) {
                $(childElemt).find("div[code]").each(function (i, divelemt) {
                    $(divelemt).hide();
                })
            }
        }
    });
}
