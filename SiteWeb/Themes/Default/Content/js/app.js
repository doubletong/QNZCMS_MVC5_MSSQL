
function resizeMenu() {
    var winH = $(window).height();
    $("#mainav").height(winH);
    $("#mainav .item").height(winH / 3);
    $(".section-celebs .item").height(winH);
}

$(function () {
    $('.menu-button').on('click', function () {
        $(this).toggleClass('is_active');
        $("#mainav").toggleClass('openav');

    });

    resizeMenu();

    $(window).resize(function () {
        resizeMenu();
    })
})