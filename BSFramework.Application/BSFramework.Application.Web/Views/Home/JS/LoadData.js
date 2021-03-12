

    //本地调用 
    function addTab(dId, href, text)  //主键id，路径，名称
    {
        $(".navbar-custom-menu>ul>li.open").removeClass("open");
        var dataId = dId;
        var menuName = text;
        top.$.cookie('currentmoduleId', dataId, { path: "/" });
        top.$.cookie('currentmoduleName', menuName, { path: "/" });
        var dataUrl = href;

        var flag = true;
        if (dataUrl == undefined || $.trim(dataUrl).length == 0) {
            return false;
        }
        /*20181122 xjl  主页点击，打开iframe */
        parent.$('.menuTab').each(function () {
            //parent.$(this).data('id') == 页面地址 ；
            //循环判断所有标签，是否有相同地址的页面存在，有则打开，没有则创建
            // * 原有模块，路径均无参数，主页跳转页面需增加参数，所以不会跳转，而是重新创建一个
           
            if(parent.$(this).data('id').indexOf(dataUrl) != -1 || dataUrl.indexOf(parent.$(this).data('id')) != -1) {
                //if (parent.$(this).data('id') == dataUrl) {
                if (!parent.$(this).hasClass('active')) {  //要打开的页面没有选中，隐藏特效并刷新页面（首页点击，此项必定未false 所以一定会执行）
                    parent.$(this).addClass('active').siblings('.menuTab').removeClass('active');
                    parent.$.tab.scrollToTab(this);
                    parent.$('.mainContent .desktop_iframe').each(function () {

                        if (parent.$(this).data('id').indexOf(dataUrl) != -1 || dataUrl.indexOf(parent.$(this).data('id')) != -1) {
                            parent.$(this).show().siblings('.desktop_iframe').hide();
                            parent.$(this).context.src = dataUrl; //刷新iframe
                            return false;
                        }
                    });
                }
                flag = false;
                return false;
            }
        });
        if (flag) {
            debugger;
            var str = '<a href="javascript:;" class="active menuTab" data-id="' + dataUrl + '" mid="' + dataId + '">' + menuName + ' <i class="fa fa-remove"></i></a>';
            parent.$('.menuTab').removeClass('active');

            var str1 = '<iframe class="desktop_iframe" id="iframe' + dataId + '" name="iframe' + dataId + '"  width="100%" height="' + parent.$('#mainContent').find('iframe.desktop_iframe').eq(0).height() + '" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '" seamless></iframe>';
            parent.$('.mainContent').find('iframe.desktop_iframe').hide();
            parent.$('.mainContent').append(str1);
            // Loading(true);
            //parent.$('.mainContent iframe:visible').load(function () {
            //    Loading(false);
            //});
            parent.$('.menuTabs .page-tabs-content').append(str);
            //parent.$.tab.scrollToTab(parent.$('.menuTab.active'));
            return false;
        }
    }

//双控调用 ， iframe、menutab样式不同，所以无法跳转
//function addTab(dId, href, text)  //主键id，路径，名称
//{

        
//    $(".navbar-custom-menu>ul>li.open").removeClass("open");
//    var dataId = dId;
//    var menuName = text;
//    top.$.cookie('currentmoduleId', dataId, { path: "/" });
//    top.$.cookie('currentmoduleName', menuName, { path: "/" });
//    var dataUrl = top.location.origin + href;

//    var flag = true;
//    if (dataUrl == undefined || $.trim(dataUrl).length == 0) {
//        return false;
//    }
//    /*20181122 xjl  主页点击，打开iframe */
//    parent.$('.menuTab').each(function () {
//        //parent.$(this).data('id') == 页面地址 ；
//        //循环判断所有标签，是否有相同地址的页面存在，有则打开，没有则创建
//        // * 原有模块，路径均无参数，主页跳转页面需增加参数，所以不会跳转，而是重新创建一个
            
//        if ($(this).data('id').indexOf(dataUrl) != -1 || dataUrl.indexOf($(this).data('id')) != -1) {
//            //if (parent.$(this).data('id') == dataUrl) {

//            if (!parent.$(this).hasClass('active')) {  //要打开的页面没有选中，隐藏特效并刷新页面（首页点击，此项必定未false 所以一定会执行）
//                parent.$(this).addClass('active').siblings('.menuTab').removeClass('active');
//                parent.$.tab.scrollToTab(this);
//                parent.$('.mainContent .LRADMS_iframe').each(function () {

//                    if (parent.$(this).data('id').indexOf(dataUrl) != -1 || dataUrl.indexOf(parent.$(this).data('id')) != -1) {
//                        parent.$(this).show().siblings('.LRADMS_iframe').hide();
//                        parent.$(this).context.src = dataUrl; //刷新iframe
//                        return false;
//                    }
//                });
//            }
//            flag = false;
//            return false;
//        }
//    });
//    if (flag) {
            
//        var str = '<a href="javascript:;" class="active menuTab" data-id="' + dataUrl + '" mid="' + dataId + '">' + menuName + ' <i class="fa fa-remove"></i></a>';
//        parent.$('.menuTab').removeClass('active');

//        var str1 = '<iframe class="LRADMS_iframe" id="iframe' + dataId + '" name="iframe' + dataId + '"  width="100%" height="' + parent.$('.mainContent').find('iframe.LRADMS_iframe').eq(0).height() + '" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '" seamless></iframe>';
//        parent.$('.mainContent').find('iframe.LRADMS_iframe').hide();
//        parent.$('.mainContent').append(str1);
//        // Loading(true);
//        //parent.$('.mainContent iframe:visible').load(function () {
//        //    Loading(false);
//        //});
//        parent.$('.menuTabs .page-tabs-content').append(str);
//        //parent.$.tab.scrollToTab(parent.$('.menuTab.active'));
//        return false;
//    }
//}
function scrollToTab(t) {
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

}

