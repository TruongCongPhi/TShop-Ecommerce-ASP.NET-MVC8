const colors = [];

function createBadge(color) {
    const newBadgeContainer = document.createElement('div');
    newBadgeContainer.className = 'badge-container';

    const newBadge = document.createElement('span');
    newBadge.className = 'badge me-1 py-2 px-3';
    newBadge.style.backgroundColor = color.ColorCode;
    newBadge.textContent = color.ColorName;

    const removeBadgeButton = document.createElement('span');
    removeBadgeButton.className = 'remove-badge';
    removeBadgeButton.innerHTML = '&times;';

    removeBadgeButton.addEventListener('click', function () {
        const badgeContainer = document.getElementById('badgeContainer');
        badgeContainer.removeChild(newBadgeContainer);
        // Xóa màu khỏi mảng colors
        const index = colors.findIndex(c => c.ColorCode === color.ColorCode && c.ColorName === color.ColorName);
        if (index > -1) {
            colors.splice(index, 1);
        }
    });

    newBadgeContainer.appendChild(newBadge);
    newBadgeContainer.appendChild(removeBadgeButton);

    

    return newBadgeContainer;
}

function initColors() {
    document.getElementById('addColorButton').addEventListener('click', function () {
        // Lấy màu và tên màu từ input
        const colorInput = document.getElementById('exampleColorInput');
        const selectedColor = colorInput.value;
        const colorNameInput = document.getElementById('colorNameInput');
        const colorName = colorNameInput.value.trim() || selectedColor;

        // Lưu màu đã thêm vào mảng colors
        const newColor = { ColorIdata1: null, ColorCode: selectedColor, ColorName: colorName };
        colors.push(newColor);

        // Tạo badge mới
        const badgeContainer = document.getElementById('badgeContainer');
        const newBadgeContainer = createBadge(newColor);

        // Thêm badge mới vào danh sách trước phần tử button
        badgeContainer.insertBefore(newBadgeContainer, badgeContainer.querySelector('.position-relative'));

        // Xóa nội dung của các input sau khi thêm badge
        colorNameInput.value = '';
    });

    // Hiển thị các màu hiện có
    const badgeContainer = document.getElementById('badgeContainer');
    colors.forEach(color => {
        const newBadgeContainer = createBadge(color);
        badgeContainer.insertBefore(newBadgeContainer, badgeContainer.querySelector('.position-relative'));
    });

    console.log(colors);
}

// Khởi tạo sự kiện sau khi nội dung được tải
document.addEventListener('DOMContentLoaded', function () {
    initColors();
});
