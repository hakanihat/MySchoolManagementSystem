document.getElementById('uploadPicture').onclick = function () {
    document.getElementById('Input_PictureFile').click();
};

document.getElementById('Input_PictureFile').onchange = function () {
    var input = this;
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            document.getElementById('profilePicturePreview').src = e.target.result;
            document.getElementById('Input_PictureUrl').value = e.target.result;
            document.getElementById('profilePicturePreview').style.display = 'block';
        };

        reader.readAsDataURL(input.files[0]);
    }
};