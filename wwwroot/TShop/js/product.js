$(document).ready(function () {
    // Xử lý sự kiện khi click vào màu
    $('.product__color__select .color__show').click(function () {
        var colorId = $(this).data('color-id');

        // Cập nhật ảnh sản phẩm theo màu đã chọn (nếu có ảnh màu chính)
        var imageUrl = $(this).data('main-image-url');
        if (imageUrl) {
            $(this).closest('.product__item__card').find('.img__product').attr('src', imageUrl);
        }

        // Lấy dữ liệu từ data-variant
        var variants = $(this).closest('.product__item__card').find('.product__size__select').data('variant');
        // Cập nhật các size tương ứng với màu đã chọn
        var sizesHtml = '<span>Thêm nhanh vào giỏ hàng</span>';
        variants.forEach(variant => {
            if (variant.ColorId === colorId) {
                variant.VariantDetails.forEach(vd => {
                    sizesHtml += `<div class="size__show ${vd.Quantity === 0 ? "out__of__stock" : ""}" data-variantdetailid="${vd.VariantDetailId}">${vd.SizeName}</div>`;
                });
            }
        });
        $(this).closest('.product__item__card').find('.product__size__select').html(sizesHtml);

        // Thêm class selected vào div màu được chọn và loại bỏ class này khỏi các div khác
        $(this).siblings().removeClass('selected');
        $(this).addClass('selected');

        // click vào size thêm nhanh vào giỏ hàng
        $(this).closest('.product__item__card').find('.size__show').click(function () {
            if ($(this).hasClass('out__of__stock')) {
                return; 
            }

            var variantDetailId = $(this).data("variantdetailid");
            $.ajax({
                type: "POST",
                url: "/api/add/cart",
                data: {
                    variantDetailId: variantDetailId
                },
                success: function (result) {
                    $.ajax({
                        url: '/api/cart/count',
                        type: "GET",
                        success: function (result) {
                            if (result.success) {
                                $('.number-cart').html(result.count);
                            };
                            showToast("Đã thêm sản phẩm vào giỏ hàng");

                        }
                    });
                    $('#offcanvasCart').html(result);
         
                    $.when(
                        $.getScript('/tshop/js/product.js'),
                        $.getScript('/tshop/js/main.js'),
                    );
         
                }
            });
        });
    });

    // Chọn màu đầu tiên khi trang được tải
    $('.product__color__select').each(function () {
        var firstColor = $(this).find('.color__show').first();
        firstColor.trigger('click');
    });

    // Xử lý sự kiện hover vào sản phẩm
    $('.product__item__card').hover(
        function () {
            var addCart = $(this).find('.add-cart');
            var addSize = $(this).find('.product__size__select');

            // Hiển thị nút "Thêm giỏ hàng" khi hover nếu phần chọn size không hiển thị
            if (addSize.css('display') !== 'flex') {
                addCart.css('display', 'flex');
            }
        },
        function () {
            var addCart = $(this).find('.add-cart');
            var addSize = $(this).find('.product__size__select');

            // Nếu phần chọn size đang hiển thị, ẩn nó đi và hiển thị nút "Thêm giỏ hàng"
            if (addSize.css('display') === 'flex') {
                addSize.css('display', 'none');
                addCart.css('display', 'flex');
            } else {
                // Ẩn nút "Thêm giỏ hàng" khi không hover nếu phần chọn size không hiển thị
                addCart.css('display', 'none');
            }
        }
    );

    // Xử lý sự kiện khi click vào nút "Thêm giỏ hàng"
    $('.add-cart').click(function () {
        // Tìm container cha gần nhất và các phần tử con của nó
        var $parent = $(this).closest('.product__item__pic');

        // Ẩn nút "Thêm giỏ hàng"
        $(this).css('display', 'none');

        // Hiển thị phần chọn size
        $parent.find('.product__size__select').css('display', 'flex');
    });


    $('.pro-qty').on('click', '.qtybtn', function () {
        var productId = $(this).parent().data("productid");
        var colorId = $(this).parent().data("colorid");
        var size = $(this).parent().data("size");
        var $input = $(this).parent().find('input');
        var amount = parseInt($input.val());
        var $this = $(this).closest('.cart-item');

        if (amount == 0) {
            $('#deleteModal').modal('show');
            $('#confirmDelete').off('click').on('click', function () {
                $('#deleteModal').modal('hide');
                updateCart(productId, colorId, size, amount);
                $this.remove();

            });
            $('#deleteModal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
                if (amount === 0) {
                    $input.val(1); 
                }
            });
        } else {
            updateCart(productId, colorId, size, amount);
        };
    });
    $('.cart-item-remove').click(function () {
        var $this = $(this).closest('.cart-item');
        var productId = $this.find('.pro-qty').data("productid");
        var colorId = $this.find('.pro-qty').data("colorid");
        var size = $this.find('.pro-qty').data("size");

        $('#deleteModal').modal('show');

        // Khi người dùng xác nhận xóa
        $('#confirmDelete').off('click').on('click', function () {
            $('#deleteModal').modal('hide');
            $.ajax({
                type: "POST",
                url: "/ShoppingCart/RemoveCartItem",
                data: {
                    productId: productId,
                    colorId: colorId,
                    size: size
                },
                success: function (result) {
                    if (result.success) {
                        $this.remove();
                        showToast(result.message);
                        $('.total-price-base').html(formatCurrencyVND(result.totalPriceBase));
                        $('.total-discount').html('-' + formatCurrencyVND(result.totalDiscount));
                        $('.total-discount-main').html('(tiết kiệm ' + formatCurrencyVND(result.totalDiscount) + ')');
                        $('.total-price-main').html(formatCurrencyVND(result.totalPriceMain));
                        $('.number-cart').html(result.count);
                        if (result.totalPriceMain === 0) {
                            $('#offcanvasCart .offcanvas-body').html('<div>Không có sản phẩm nào trong giỏ hàng</div>');
                            $('#offcanvasCart .offcanvas-footer').remove();

                        }
                    } else {
                        showToast(result.message, 'error');
                    }
                },
                error: function (xhr, status, error) {        
                    showToast("Có lỗi xảy ra khi xóa sản phẩm.", 'error');
                }
            });
            $('#deleteModal').off('hidden.bs.modal').on('hidden.bs.modal', function () {
   
            });
        });
    });



});

function updateCart(productId, colorId, size, amount ) {
    $.ajax({
        type: "POST",
        url: "/api/cart/update",
        data: {
            productId: productId,
            colorId: colorId,
            size: size,
            amount: amount
        },
        success: function (result) {
            if (result.success) {
                showToast(result.message);
                $('.total-price-base').html(formatCurrencyVND(result.totalPriceBase));
                $('.total-discount').html('-' + formatCurrencyVND(result.totalDiscount));
                $('.total-discount-main').html('(tiết kiệm ' + formatCurrencyVND(result.totalDiscount) + ')');
                $('.total-price-main').html(formatCurrencyVND(result.totalPriceMain));
                $('.number-cart').html(result.count);
                if (result.totalPriceMain === 0) {
                    $('#offcanvasCart .offcanvas-body').html('<div>Không có sản phẩm nào trong giỏ hàng</div>');
                    $('#offcanvasCart .offcanvas-footer').remove();

                };

            } else {
                showToast(result.message, 'error');
            };

        },
        error: function () {
            showToast("Có lỗi xảy ra khi cập nhật số lượng.", 'error');
        }
    });
}
function formatCurrencyVND(number) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(number);
}
function showToast(message,type = 'success', duration = 3000) {
    var toastHtml = `
        <div class="toast align-items-center text-bg-${type} border-0" role="alert" aria-live="assertive" aria-atomic="true" data-bs-autohide="true" data-bs-delay="${duration}">
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>`;
    var $toast = $(toastHtml);
    $('#toast-container').append($toast);
    $toast.toast('show');

    // Xóa thông báo sau khi đã hiển thị xong
    $toast.on('hidden.bs.toast', function () {
        $(this).remove();
    });
}

