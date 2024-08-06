

function initImages() {
   
        const colorFileInputs = document.querySelectorAll('[id^="formFileSm-"]');

        colorFileInputs.forEach(fileInput => {
            const colorCode = fileInput.id.replace('formFileSm-', '');
            const uploadedImageDiv = document.getElementById('uploaded_image-' + colorCode);

            fileInput.addEventListener('change', function (event) {
                const file = event.target.files[0];
                if (file) {
                    const reader = new FileReader();
                    reader.onload = function (e) {
                        const imageElement = document.createElement('img');
                        imageElement.src = e.target.result;
                        imageElement.style.maxWidth = '100%';
                        imageElement.style.maxHeight = '300px';
                        uploadedImageDiv.innerHTML = '';
                        uploadedImageDiv.appendChild(imageElement);
                    };
                    reader.readAsDataURL(file);
                }
            });
        });
        
    
   

}
document.addEventListener('DOMContentLoaded', function () {
    initImages();
});
//size
function addSizeRow(colorCode) {
    var tbody = document.getElementById('size-rows-' + colorCode);
    var newRow = document.createElement('tr');
    newRow.classList.add('btn-reveal-trigger');

    newRow.innerHTML = `
            <td class="ps-0 py-2">
                <input class="form-control form-control-sm" type="text" placeholder="Nhập kích thước" name="sizes[${colorCode}][]" />
            </td>
            <td class="align-middle py-2">
                <input class="form-control form-control-sm" type="text" placeholder="0.00đ" name="prices[${colorCode}][]" />
            </td>
            <td class="phone align-middle white-space-nowrap py-2">
                <input class="form-control form-control-sm" type="number" placeholder="0" name="quantities[${colorCode}][]" />
            </td>
            <td class="align-middle white-space-nowrap py-2">
                <div class="dropdown font-sans-serif position-static">
                    <button class="btn btn-link btn-sm" type="button" onclick="removeSizeRow(this)">
                    <span class="fas fa-times-circle text-danger" data-fa-transform="shrink-2"></span>
                    </button>
                </div>
            </td>
        `;
    tbody.appendChild(newRow);
}

function removeSizeRow(button) {
    var row = button.closest('tr');
    row.parentNode.removeChild(row);
}

