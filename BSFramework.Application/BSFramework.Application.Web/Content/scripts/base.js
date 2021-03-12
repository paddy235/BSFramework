

//$(function () {
    
//    $(window).resize(function () {
//        computeH();
//    }).trigger('resize');
//   // console.log( computeH )
//})
//function computeH(){
//    var winH = $(window).height();
//    $('.main-content').css('height', winH - 100).css('overflow', 'auto');
//    $('.container-fluid').css('min-height', winH - 140);
//}

$(function () {
    $('.main-content').niceScroll({
        autohidemode: false
    });

    

    $(window).resize(function () {
        computeH();
    }).trigger('resize');
})

//设置body部分的高度
function computeH(){
    var winH = $(window).height();
    $('.container-fluid .my-body').css('min-height', winH - 200);
}

