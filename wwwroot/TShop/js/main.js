/*  ---------------------------------------------------
    Template Name: Male Fashion
    Description: Male Fashion - ecommerce teplate
    Author: Colorib
    Author URI: https://www.colorib.com/
    Version: 1.0
    Created: Colorib
---------------------------------------------------------  */

'use strict';

(function ($) {
    

    /*------------------
        Product Slider
    --------------------*/
    $(".product-slider-3").owlCarousel({
        loop: true,
        margin: 30,
        nav: true,
        items: 3,
        dots: true,
        navText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: false,
        responsive: {
            0: {
                items: 1,
            },
            200: {
                items: 2,
            },
            576: {
                items: 3,
            },
            900: {
                items: 3,
            },
        }
    });
    $(".product-slider-4").owlCarousel({
        loop: true,
        margin: 30,
        nav: true,
        items: 4,
        dots: true,
        navText: ['<i class="fa fa-angle-left" aria-hidden="true"></i>', '<i class="fa fa-angle-right" aria-hidden="true"></i>'],
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: false,
        responsive: {
            0: {
                items: 1,
            },
            200: {
                items: 2,
            },
            576: {
                items: 3,
            },
            900: {
                items: 4,
            },
        }
    });
    /*------------------
        Background Set
    --------------------*/
    $('.set-bg').each(function () {
        var bg = $(this).data('setbg');
        $(this).css('background-image', 'url(' + bg + ')');
    });

    //Search Switch
    $('.search-switch').on('click', function () {
        $('.search-model').fadeIn(400);
    });

    $('.search-close-switch').on('click', function () {
        $('.search-model').fadeOut(400, function () {
            $('#search-input').val('');
        });
    });

    /*------------------
		Navigation
	--------------------*/
    $(".mobile-menu").slicknav({
        prependTo: '#mobile-menu-wrap',
        allowParentLinks: true
    });

    /*------------------
        Accordin Active
    --------------------*/
    $('.collapse').on('shown.bs.collapse', function () {
        $(this).prev().addClass('active');
    });

    $('.collapse').on('hidden.bs.collapse', function () {
        $(this).prev().removeClass('active');
    });

    //Canvas Menu
    $(".canvas__open").on('click', function () {
        $(".offcanvas-menu-wrapper").addClass("active");
        $(".offcanvas-menu-overlay").addClass("active");
    });

    $(".offcanvas-menu-overlay").on('click', function () {
        $(".offcanvas-menu-wrapper").removeClass("active");
        $(".offcanvas-menu-overlay").removeClass("active");
    });

    /*-----------------------
        Hero Slider
    ------------------------*/
    $(".hero__slider").owlCarousel({
        loop: true,
        margin: 0,
        items: 1,
        dots: false,
        nav: true,
        navText: ["<span class='arrow_left'><span/>", "<span class='arrow_right'><span/>"],
        animateOut: 'fadeOut',
        animateIn: 'fadeIn',
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: false
    });

    /*--------------------------
        Select
    ----------------------------*/
    $("select").niceSelect();

    /*-------------------
		Radio Btn
	--------------------- */
    $(".product__color__select label, .shop__sidebar__size label, .product__details__option__size label").on('click', function () {
        $(".product__color__select label, .shop__sidebar__size label, .product__details__option__size label").removeClass('active');
        $(this).addClass('active');
    });

    /*-------------------
        Active
    --------------------- */
    $(".menu-account li").on('click', function () {
        $(".menu-account li").removeClass('active');
        $(this).addClass('active');
    });
   

    /*-------------------
		Scroll
	--------------------- */
    $(".nice-scroll").niceScroll({
        cursorcolor: "#0d0d0d",
        cursorwidth: "5px",
        background: "#e5e5e5",
        cursorborder: "",
        autohidemode: true,
        horizrailenabled: false
    });

    /*------------------
        CountDown
    --------------------*/
    // For demo preview start
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();

    if(mm == 12) {
        mm = '01';
        yyyy = yyyy + 1;
    } else {
        mm = parseInt(mm) + 1;
        mm = String(mm).padStart(2, '0');
    }
    var timerdate = mm + '/' + dd + '/' + yyyy;
    // For demo preview end


    // Uncomment below and use your date //

    /* var timerdate = "2020/12/30" */

    $("#countdown").countdown(timerdate, function (event) {
        $(this).html(event.strftime("<div class='cd-item'><span>%D</span> <p>Days</p> </div>" + "<div class='cd-item'><span>%H</span> <p>Hours</p> </div>" + "<div class='cd-item'><span>%M</span> <p>Minutes</p> </div>" + "<div class='cd-item'><span>%S</span> <p>Seconds</p> </div>"));
    });

    /*------------------
		Magnific
	--------------------*/
    $('.video-popup').magnificPopup({
        type: 'iframe'
    });

    /*-------------------
		Quantity change
	--------------------- */
    var proQty = $('.pro-qty');
    proQty.on('click', '.qtybtn', function () {
        var $button = $(this);
        var oldValue = $button.parent().find('input').val();
        if ($button.hasClass('inc')) {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            // Don't allow decrementing below zero
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
            } else {
                newVal = 0;
            }
        }
        $button.parent().find('input').val(newVal);
    });

    var proQty = $('.pro-qty-2');
    proQty.prepend('<span class="fa fa-angle-left dec qtybtn"></span>');
    proQty.append('<span class="fa fa-angle-right inc qtybtn"></span>');
    proQty.on('click', '.qtybtn', function () {
        var $button = $(this);
        var oldValue = $button.parent().find('input').val();
        if ($button.hasClass('inc')) {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            // Don't allow decrementing below zero
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
            } else {
                newVal = 0;
            }
        }
        $button.parent().find('input').val(newVal);
    });

    /*------------------
        Achieve Counter
    --------------------*/
    $('.cn_num').each(function () {
        $(this).prop('Counter', 0).animate({
            Counter: $(this).text()
        }, {
            duration: 4000,
            easing: 'swing',
            step: function (now) {
                $(this).text(Math.ceil(now));
            }
        });
    });
   
    /*------------------
        Range Price
    --------------------*/
    const rangeInput = $(".range-input input");
    const priceInput = $(".price-input input");
    const range = $(".slider__price .progress__price");
    const priceLabelMin = $(".price-label-min");
    const priceLabelMax = $(".price-label-max");

    function formatCurrencyVND(number) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(number);
    }

    priceLabelMin.text(formatCurrencyVND(parseInt(priceLabelMin.text())));
    priceLabelMax.text(formatCurrencyVND(parseInt(priceLabelMax.text())));

    let priceGap = 1000;

    priceInput.on("input", function (e) {
        let minPrice = parseInt(priceInput.eq(0).val());
        let maxPrice = parseInt(priceInput.eq(1).val());

        if (maxPrice - minPrice >= priceGap && maxPrice <= rangeInput.eq(1).attr("max")) {
            if ($(e.target).hasClass("input-min")) {
                rangeInput.eq(0).val(minPrice);
                range.css("left", (minPrice - rangeInput.eq(0).attr("min")) / (rangeInput.eq(1).attr("max") - rangeInput.eq(0).attr("min")) * 100 + "%");
            } else {
                rangeInput.eq(1).val(maxPrice);
                range.css("right", (rangeInput.eq(1).attr("max") - maxPrice) / (rangeInput.eq(1).attr("max") - rangeInput.eq(0).attr("min")) * 100 + "%");
            }
        }
    });

    rangeInput.on("input", function (e) {
        let minVal = parseInt(rangeInput.eq(0).val());
        let maxVal = parseInt(rangeInput.eq(1).val());

        if (maxVal - minVal < priceGap) {
            if ($(e.target).hasClass("range-min")) {
                rangeInput.eq(0).val(maxVal - priceGap);
            } else {
                rangeInput.eq(1).val(minVal + priceGap);
            }
        } else {
            priceInput.eq(0).val(minVal);
            priceInput.eq(1).val(maxVal);
            priceLabelMin.text(formatCurrencyVND(parseInt(minVal)));
            priceLabelMax.text(formatCurrencyVND(parseInt(maxVal)));
            range.css("left", (minVal - rangeInput.eq(0).attr("min")) / (rangeInput.eq(1).attr("max") - rangeInput.eq(0).attr("min")) * 100 + "%");
            range.css("right", (rangeInput.eq(1).attr("max") - maxVal) / (rangeInput.eq(1).attr("max") - rangeInput.eq(0).attr("min")) * 100 + "%");

            priceLabelMin.css("left", (minVal - rangeInput.eq(0).attr("min")) / (rangeInput.eq(1).attr("max") - rangeInput.eq(0).attr("min")) * 100 + "%");
            priceLabelMax.css("right", (rangeInput.eq(1).attr("max") - maxVal) / (rangeInput.eq(1).attr("max") - rangeInput.eq(0).attr("min")) * 100 + "%");
          
        }

        
    });
    

})(jQuery);

