var control = document.getElementById("file-upload");
control.addEventListener("change", function (event) {
    var i = 0,
        files = control.files,
        len = files.length;
    document.getElementById('fileList').innerHTML = "Выбранные файлы:<br/>";
    for (; i < len; i++) {
        document.getElementById('fileList').innerHTML += files[i].name + "<br/>";
    }
}, false);