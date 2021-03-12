var contentPath = "";
(function ($) {
    tablist = {

        newTab: function (item) {
            
            var dataId = item.id;
            var dataUrl = item.url;
            var menuName = item.title;
            var flag = true;
            if (dataUrl == undefined || $.trim(dataUrl).length == 0) {
                return false;
            }
            top.$.cookie('currentmoduleId', dataId, { path: "/" });
            top.$.cookie('currentmoduleName', menuName, { path: "/" });
            $('.menuTab').each(function () {
                if ($(this).data('id') == dataUrl) {
               
                    if (!$(this).hasClass('active')) {
                        $(this).addClass('active').siblings('.menuTab').removeClass('active');
                        $.tab.scrollToTab(this);
                        $('.mainContent .desktop_iframe').each(function () {
                            if ($(this).data('id') == dataUrl) {
                                $(this).show().siblings('.desktop_iframe').hide();
                                
                                return false;
                            }
                        });
                    }
                    flag = false;
                    return false;
                }
            });
            if (flag) {
                var str = '<a href="javascript:;" class="active menuTab" data-id="' + dataUrl + '">' + menuName + ' <i class="fa fa-remove"></i></a>';
                $('.menuTab').removeClass('active');
                var str1 = '<iframe class="desktop_iframe" id="iframe' + dataId + '" name="iframe' + dataId + '"  width="100%" height="100%" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '" seamless></iframe>';
                $('.mainContent').find('iframe.desktop_iframe').hide();
                $('.mainContent').append(str1);
                Loading(true);
                $('.mainContent iframe:visible').load(function () {
                    Loading(false);
                });
                $('.menuTabs .page-tabs-content').append(str);
                $.tab.scrollToTab($('.menuTab.active'));
            }
        }
    }
    $.tab = {
        requestFullScreen: function () {
            var de = document.documentElement;
            if (de.requestFullscreen) {
                de.requestFullscreen();
            } else if (de.mozRequestFullScreen) {
                de.mozRequestFullScreen();
            } else if (de.webkitRequestFullScreen) {
                de.webkitRequestFullScreen();
            }
        },
        exitFullscreen: function () {
            var de = document;
            if (de.exitFullscreen) {
                de.exitFullscreen();
            } else if (de.mozCancelFullScreen) {
                de.mozCancelFullScreen();
            } else if (de.webkitCancelFullScreen) {
                de.webkitCancelFullScreen();
            }
        },
        refreshTab: function () {
            var currentId = $('.page-tabs-content').find('.active').attr('data-id');
            var target = $('.desktop_iframe[data-id="' + currentId + '"]');
            var url = target.attr('src');
            Loading(true);
            target.attr('src', url).load(function () {
                Loading(false);
            });
        },
        activeTab: function () {
            var dataId = $(this).attr('mid');
            var menuName = $.trim($(this).text());
            top.$.cookie('currentmoduleId', dataId, { path: "/" });
            top.$.cookie('currentmoduleName', menuName, { path: "/" });
            var currentId = $(this).data('id'); 
            if (!$(this).hasClass('active')) {
                $('.mainContent .desktop_iframe').each(function () {
                    if ($(this).data('id') == currentId) {
                        $(this).show().siblings('.desktop_iframe').hide();
                        return false;
                    }
                });
                $(this).addClass('active').siblings('.menuTab').removeClass('active');
                $.tab.scrollToTab(this);
            }
           
        },
        closeOtherTabs: function () {
            $('.page-tabs-content').children("[data-id]").find('.fa-remove').parents('a').not(".active").each(function () {
                $('.desktop_iframe[data-id="' + $(this).data('id') + '"]').remove();
                $(this).remove();
            });
            $('.page-tabs-content').css("margin-left", "0");
        },
        closeTab: function () {
    //        var i = $(this).parents(".menuTab").data("id"),
    //f = $(this).parents(".menuTab").width(),
    //r, t, u;
    //        return $(this).parents(".menuTab").hasClass("active") ? ($(this).parents(".menuTab").next(".menuTab").size() && (t = $(this).parents(".menuTab").next(".menuTab:eq(0)").data("id"), $(this).parents(".menuTab").next(".menuTab:eq(0)").addClass("active"), $("#mainContent .desktop_iframe").each(function () {
    //            if ($(this).data("id") == t) return $(this).show().siblings(".desktop_iframe").hide(), !1;
    //        }), r = parseInt($(".page-tabs-content").css("margin-left")), r < 0 && $(".page-tabs-content").animate({
    //            marginLeft: r + f + "px"
    //        }, "fast"), $(this).parents(".menuTab").remove(), $("#mainContent .desktop_iframe").each(function () {
    //            if ($(this).data("id") == i) return $(this).remove(), !1;
    //        })), $(this).parents(".menuTab").prev(".menuTab").size() && (t = $(this).parents(".menuTab").prev(".menuTab:last").data("id"), $(this).parents(".menuTab").prev(".menuTab:last").addClass("active"), $("#mainContent .desktop_iframe").each(function () {
    //            if ($(this).data("id") == t) return $(this).show().siblings(".desktop_iframe").hide(), !1;
    //        }),$(this).parents(".menuTab").remove(), $("#mainContent .desktop_iframe").each(function () {
    //            if ($(this).data("id") == i) return $(this).remove(), !1;
    //        }))) : ($(this).parents(".menuTab").remove(), $("#mainContent .desktop_iframe").each(function () {
    //            if ($(this).data("id") == i) return $(this).remove(), !1;
    //        }), $.learuntab.scrollToTab($(".menuTab.active"))), u = $(".menuTab.active").attr("data-value"), u != "" && top.$.cookie("currentmoduleId", u, {
    //            path: "/"
    //        }), !1;
            var closeTabId = $(this).parents('.menuTab').data('id');
            var currentWidth = $(this).parents('.menuTab').width();
            if ($(this).parents('.menuTab').hasClass('active')) {
                if ($(this).parents('.menuTab').next('.menuTab').size()) {
                    var activeId = $(this).parents('.menuTab').next('.menuTab:eq(0)').data('id');
                    $(this).parents('.menuTab').next('.menuTab:eq(0)').addClass('active');

                    $('.mainContent .desktop_iframe').each(function () {
                        if ($(this).data('id') == activeId) {
                            $(this).show().siblings('.desktop_iframe').hide();  
                            return false;
                        }
                    });
                    var marginLeftVal = parseInt($('.page-tabs-content').css('margin-left'));
                    if (marginLeftVal < 0) {
                        $('.page-tabs-content').animate({
                            marginLeft: (marginLeftVal + currentWidth) + 'px'
                        }, "fast");
                    }
                    $(this).parents('.menuTab').remove();
                    $('.mainContent .desktop_iframe').each(function () {
                        if ($(this).data('id') == closeTabId) {
                            $(this).remove();  
                            return false;
                        }
                    });
                }
                if ($(this).parents('.menuTab').prev('.menuTab').size()) {
                    var activeId = $(this).parents('.menuTab').prev('.menuTab:last').data('id');
                    $(this).parents('.menuTab').prev('.menuTab:last').addClass('active');
                    $('.mainContent .desktop_iframe').each(function () {
                        if ($(this).data('id') == activeId) {
                            $(this).show().siblings('.desktop_iframe').hide();
                            var marginLeftVal = parseInt($('.page-tabs-content').css('margin-left'));
                            if (marginLeftVal < 0) {
                                $('.page-tabs-content').animate({
                                    marginLeft: (marginLeftVal + currentWidth) + 'px'
                                }, "fast");
                            }
                           
                            return false;
                        }
                    });
                    $(this).parents('.menuTab').remove();
                    $('.mainContent .desktop_iframe').each(function () {
                        if ($(this).data('id') == closeTabId) {
                            $(this).remove();
                            return false;
                        }
                    });
                }
            }
            else {
                $(this).parents('.menuTab').remove();
                $('.mainContent .desktop_iframe').each(function () {
                    if ($(this).data('id') == closeTabId) {
                        $(this).remove();
                        return false;
                    }
                });
                $.tab.scrollToTab($('.menuTab.active'));  
            }
            return false;
        },
        addTab: function (dId, href, text) {
            $(".navbar-custom-menu>ul>li.open").removeClass("open");
            var dataId = href == undefined ? $(this).attr('data-id') : dId;
            var menuName = text == undefined ? $.trim($(this).text()) : text;
            if (dataId != "") {
                top.$.cookie('currentmoduleId', dataId, { path: "/" });
                top.$.cookie('currentmoduleName', menuName, { path: "/" });
            }
            var dataUrl = href == undefined ? $(this).attr('href') : href;
           
            var flag = true;
            if (dataUrl == undefined || $.trim(dataUrl).length == 0) {
                return false;
            }
            $('.menuTab').each(function () {
                if ($(this).data('id') == dataUrl) {
                    //已打开的tab，路径对比
                //if ($(this).data('id').indexOf(dataUrl) != -1 || dataUrl.indexOf($(this).data('id')) != -1) {
                    if (!$(this).hasClass('active')) {  //是否选中，主菜单点击，此项可能未false，所以增加else选项刷新页面
                        $(this).addClass('active').siblings('.menuTab').removeClass('active');
                        $.tab.scrollToTab(this);
                        $('.mainContent .desktop_iframe').each(function () {
                            if ($(this).data('id') == dataUrl) {
                           // if ($(this).data('id').indexOf(dataUrl) != -1 || dataUrl.indexOf($(this).data('id')) != -1) {
                                $(this).show().siblings('.desktop_iframe').hide();
                                $(this).context.src = dataUrl;
                                return false;
                            }
                        });
                    } else   // 此页面已经打开，直接刷新  20181122 xjl
                    {
                        $('.mainContent .desktop_iframe').each(function () {
                            //if ($(this).data('id') == dataUrl) {
                            if ($(this).data('id').indexOf(dataUrl) != -1 || dataUrl.indexOf($(this).data('id')) != -1) {
                               // $(this).show().siblings('.desktop_iframe').hide();
                                $(this).context.src = dataUrl;
                                return false;
                            }
                        });
                    }
                    flag = false;
                    return false;
                }
            });
            if (flag) {
                var str = '<a href="javascript:;" class="active menuTab" data-id="' + dataUrl + '" mid="'+dataId+'">' + menuName + ' <i class="fa fa-remove"></i></a>';
                $('.menuTab').removeClass('active');
                var str1 = '<iframe class="desktop_iframe" id="iframe' + dataId + '" name="iframe' + dataId + '"  width="100%" height="' + $('#mainContent').find('iframe.desktop_iframe').eq(0).height() + 'px" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '" seamless></iframe>';
                $('.mainContent').find('iframe.desktop_iframe').hide();
                $('.mainContent').append(str1);
                Loading(true);
                $('.mainContent iframe:visible').load(function () {
                    Loading(false);
                });
                $('.menuTabs .page-tabs-content').append(str);
                $.tab.scrollToTab($('.menuTab.active'));
            }
            return false;
        },
        scrollTabRight: function () {
            var f = Math.abs(parseInt($(".page-tabs-content").css("margin-left"))),
     e = $.tab.calSumWidth($(".lea-tabs").children().not(".menuTabs")),
     u = $(".lea-tabs").outerWidth(!0) - e,
     r = 0,
     t, i;
            if ($(".page-tabs-content").width() < u) return !1;
            for (t = $(".menuTab:first"), i = 0; i + $(t).outerWidth(!0) <= f;) i += $(t).outerWidth(!0), t = $(t).next();
            for (i = 0; i + $(t).outerWidth(!0) < u && t.length > 0;) i += $(t).outerWidth(!0), t = $(t).next();
            r = $.tab.calSumWidth($(t).prevAll());
            r > 0 && $(".page-tabs-content").animate({
                marginLeft: 0 - r + "px"
            }, "fast");
        },
        scrollTabLeft: function () {
            var f = Math.abs(parseInt($(".page-tabs-content").css("margin-left"))),
                  e = $.tab.calSumWidth($(".lea-tabs").children().not(".menuTabs")),
                  r = $(".lea-tabs").outerWidth(!0) - e,
                  u = 0,
                  t, i;
            if ($(".page-tabs-content").width() < r) return !1;
            for (t = $(".menuTab:first"), i = 0; i + $(t).outerWidth(!0) <= f;) i += $(t).outerWidth(!0), t = $(t).next();
            if (i = 0, $.tab.calSumWidth($(t).prevAll()) > r) {
                while (i + $(t).outerWidth(!0) < r && t.length > 0) i += $(t).outerWidth(!0), t = $(t).prev();
                u = $.tab.calSumWidth($(t).prevAll())
            }
            $(".page-tabs-content").animate({
                marginLeft: 0 - u + "px"
            }, "fast");
        },
        scrollToTab: function (t) {

            var f = $.tab.calSumWidth($(t).prevAll()),
             e = $.tab.calSumWidth($(t).nextAll()),
             o = $.tab.calSumWidth($(".lea-tabs").children().not(".menuTabs")),
    r = $(".lea-tabs").outerWidth(!0) - o,
    i = 0,
    u;
            if ($(".page-tabs-content").outerWidth() < r) i = 0;
            else if (e <= r - $(t).outerWidth(!0) - $(t).next().outerWidth(!0)) {
                if (r - $(t).next().outerWidth(!0) > e)
                    for (i = f, u = t; i - $(u).outerWidth() > $(".page-tabs-content").outerWidth() - r;) i -= $(u).prev().outerWidth(), u = $(u).prev()
            } else f > r - $(t).outerWidth(!0) - $(t).prev().outerWidth(!0) && (i = f - $(t).prev().outerWidth(!0));
            $(".page-tabs-content").animate({
                marginLeft: 0 - i + "px"
            }, "fast");
 
        },
        calSumWidth: function (element) {
            var width = 0;
            $(element).each(function () {
                width += $(this).outerWidth(true);
            });
            return width;
        },
        init: function () {
            
            $('.menuItem').on('click', $.tab.addTab);
            $('.menuTabs').on('click', '.menuTab i', $.tab.closeTab);
            $('.menuTabs').on('click', '.menuTab', $.tab.activeTab);
            $('.tabLeft').on('click', $.tab.scrollTabLeft);
            $('.tabRight').on('click', $.tab.scrollTabRight);
            $('.tabReload').on('click', $.tab.refreshTab);
            $('.tabCloseCurrent').on('click', function () {
                $('.page-tabs-content').find('.active i').trigger("click");
            });
            $('.tabCloseAll').on('click', function () {
                $('.page-tabs-content').children("[data-id]").find('.fa-remove').each(function () {
                    $('.desktop_iframe[data-id="' + $(this).data('id') + '"]').remove();
                    $(this).parents('a').remove();
                });
                $('.page-tabs-content').children("[data-id]:first").each(function () {
                    $('.desktop_iframe[data-id="' + $(this).data('id') + '"]').show();
                    $(this).addClass("active");
                });
                $('.page-tabs-content').css("margin-left", "0");
            });
            $('.tabCloseOther').on('click', $.tab.closeOtherTabs);
            $('.fullscreen').on('click', function () {
                if (!$(this).attr('fullscreen')) {
                    $(this).attr('fullscreen', 'true');
                    $.tab.requestFullScreen();
                } else {
                    $(this).removeAttr('fullscreen')
                    $.tab.exitFullscreen();
                }
            });
        }
    };
    $.index = {
        load: function () {
            $("body").removeClass("hold-transition");  
            $("#content-wrapper").find('.mainContent').height($(window).height() - 90);
            $(".desktop_iframe").height($(window).height() -132);
            $(window).resize(function (e) {
                $("#content-wrapper").find('.mainContent').height($(window).height() - 90);
                $(".desktop_iframe").height($(window).height() -132);
            });
            $(".sidebar-toggle").click(function () {
                if (!$("body").hasClass("sidebar-collapse")) {
                    $("body").addClass("sidebar-collapse");
                } else {
                    $("body").removeClass("sidebar-collapse");
                }
            })
            $(window).load(function () {
                window.setTimeout(function () {
                    $('#ajax-loader').fadeOut();
                    Loading(false);
                }, 300);
            });
        },
        jsonWhere: function (data, action) {
            if (action == null) return;
            var reval = new Array();
            $(data).each(function (i, v) {
                if (action(v)) {
                    reval.push(v);
                }
            })
            return reval;
        },
        loadMenu: function () {
            var data = authorizeMenuData; 
            var _html = "";
            $.each(data, function (i) {
                var row = data[i];
                if (row.ParentId == "0") {
                    if (i == 0) {
                        _html += '<li class="treeview">';
                    } else {
                        _html += '<li class="treeview">';
                    }
                    if (row.UrlAddress != null && row.UrlAddress != undefined) {
                        if (row.UrlAddress.length>2) {
                            _html += '<a class=" menuItem menuiframe" data-id="' + row.ModuleId + '" href="' + contentPath + row.UrlAddress + '" target="iframe_content">'
                        }
                        else {
                            _html += '<a>'
                        }
                    } else {
                        _html += '<a>'
                    }
                    _html += '<i class="' + row.Icon + ' firstIcon"></i><span>' + row.FullName + '</span>'
                    _html += '</a>'
                    var childNodes = $.index.jsonWhere(data, function (v) { return v.ParentId == row.ModuleId });
                    if (childNodes.length > 0) {
                        _html += '<div class="popover-menu" style="display:none;"><div class="arrow"><em></em><span></span></div><ul class="treeview-menu">';
                        $.each(childNodes, function (i) {
                            var subrow = childNodes[i];
                            var subchildNodes = $.index.jsonWhere(data, function (v) { return v.ParentId == subrow.ModuleId });
                            _html += '<li>';
                            if (subchildNodes.length > 0) {
                                _html += '<a class="menuTreeItem menuItem"><i class="' + subrow.Icon + ' firstIcon"></i>' + subrow.FullName + '';
                                _html += '<i class="fa fa-angle-right pull-right"></i></a>';
                                _html += '<div class="popover-menu-sub"><ul class="treeview-menu">';
                                $.each(subchildNodes, function (i) {
                                    var subchildNodesrow = subchildNodes[i];
                                    _html += '<li><a class="menuItem menuItem" data-id="' + subchildNodesrow.ModuleId + '" href="' +contentPath+ subchildNodesrow.UrlAddress + '" target="iframe_content"><i class="' + subchildNodesrow.Icon + ' firstIcon"></i>' + subchildNodesrow.FullName + '</a></li>';
                                });
                                _html += '</ul></div>';

                            } else {
                                _html += '<a class="menuItem menuiframe" data-id="' + subrow.ModuleId + '" href="' +contentPath+ subrow.UrlAddress + '" target="iframe_content"><i class="' + subrow.Icon + ' firstIcon"></i>' + subrow.FullName + '</a>';
                            }
                            _html += '</li>';
                        });
                        _html += '</ul></div>';
                    }
                    _html += '</li>'
                }
            });
            $("#top-menu").append(_html);
            $("#top-menu>.treeview").hover(function () {
                var t = $(this),
                    i = t.find(".popover-moreMenu"),
                    r;
                t.addClass("active");
                i.length > 0 ? (i.slideDown(150), $(i.find(".treeview>a")[0]).trigger("click")) : (r = t.find(".popover-menu"), r.slideDown(150))
            }, function () {
                var t = $(this),
                    r = t.find(".popover-menu"),
                    i = t.find(".popover-moreMenu");
                i.length == 0 ? r.slideUp(50) : i.hide();
                t.removeClass("active")
            });
            $(".popover-menu>ul>li").hover(function () {
                var t = $(this),
                    f;
                if (t.parents(".moresubmenu").length == 0) {
                    var e = $(window).width(),
                        r = $(window).height(),
                        i = t.find(".popover-menu-sub"),
                        u = i.height();
                    e - t.offset().left - 154 < 152 && i.css("left", "-156px");
                    u - 10 + t.offset().top > r && (f = u - 10 + t.offset().top - r + 46, i.css("margin-top", "-" + f + "px"));
                    t.addClass("active");
                    i.slideDown(150)
                }
            }, function () {
                var t = $(this),
                    i;
                t.parents(".moresubmenu").length == 0 && (i = t.find(".popover-menu-sub"), t.removeClass("active"), i.css("margin-top", "-46px"), i.slideUp(50))
            });
        },
        indexOut: function () {
            dialogConfirm("注：您确定要安全退出本次登录吗？", function (r) {
                if (r) {
                    Loading(true, "正在安全退出...");
                    window.setTimeout(function () {
                        $.ajax({
                            url: contentPath + "/Login/OutLogin",
                            type: "post",
                            dataType: "json",
                            success: function (data) {
                                window.location.href = contentPath + "/Login/Index";
                            }
                        });
                    }, 500);
                }
            });
        }
    };
    $(function () {
        $.index.loadMenu();
        $.tab.init();
        $.index.load()
    });
})(jQuery);