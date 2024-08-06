$(document).ready(function () {
    var sortOption = '';
    var sizeIds = [];
    var colorIds = [];
    var priceMin = $(".range-min").val();
    var priceMax = $(".range-max").val();
    var page = 1;

    $('.shop__product__option__right select').change(function () {
        sortOption = $(this).val();
        applyFilters();
    });

    $(".shop__sidebar__size .size__show").click(function () {
        var sizeSelect = parseInt($(this).data("sizeid"));

        $(this).toggleClass('selected');

        var index = sizeIds.indexOf(sizeSelect);
        if (index === -1) {
            sizeIds.push(sizeSelect);
        } else {
            sizeIds.splice(index, 1);
        }
        applyFilters();
    });

    $(".shop__sidebar__color .color__form").click(function () {
        var colorId = $(this).data("colorid");

        $(this).toggleClass('selected');

        var index = colorIds.indexOf(colorId);
        if (index === -1) {
            colorIds.push(colorId);
        } else {
            colorIds.splice(index, 1);
        }
        applyFilters();
    });
    $('.range-min, .range-max').on('change', function () {
        priceMin = $('.range-min').val();
        priceMax = $('.range-max').val();
        applyFilters();

    });
    $("#load-more").click(function () {
        loadProducts(page + 1, false);
    });

    function applyFilters() {
        page = 1;
        loadProducts(page, true);
    }

    function loadProducts(pageNumber, isLoad) {
        var categoryId = $("#load-more").data("categoryid");
        $.ajax({
            type: "POST",
            url: "loadmoreproducts",
            data: {
                page: pageNumber,
                categoryId: categoryId,
                sortOption: sortOption,
                sizeIds: sizeIds.map(Number),
                colorIds: colorIds,
                priceMin: priceMin,
                priceMax: priceMax
            },
            success: function (response) {
                if (isLoad) {
                    $(".grid-container").html(response);
                } else {
                    $(".grid-container").append(response);
                }
                page = pageNumber + 1;

                $.when(
                    $.getScript('/tshop/js/product.js'),
                );
            },
            error: function (xhr, status, error) {
                console.error("Xảy ra lỗi: " + error);
            }
        });
    }



});