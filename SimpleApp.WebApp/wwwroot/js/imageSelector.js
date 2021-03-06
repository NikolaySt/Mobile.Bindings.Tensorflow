var ImageSelector = function () {
    this.openPicker = async function (instance) {
        document.getElementById('imagePicker').click();
    }

    this.loadImage = async function (instance, callback) {
        var files = document.getElementById('imagePicker').files
        var reader = new FileReader();
        var _instance = instance;
        var _callback = callback;
        reader.onload = async function (e) {
            if (_callback)
                await _instance.invokeMethodAsync(_callback, e.target.result);
        };
        reader.readAsDataURL(files[0]);
    }

    this.applyFilter = async function (instance, callback) {
        var reader = new FileReader();
        var _instance = instance;
        var _callback = callback;
        reader.onload = async function (e) {
            if (_callback)
                await _instance.invokeMethodAsync(_callback, e.target.result);
        };
        reader.readAsDataURL(files[0]);
    }

    //function chunkSubstr(str, size) {
    //    const numChunks = Math.ceil(str.length / size)
    //    const chunks = new Array(numChunks)
    //    for (let i = 0, o = 0; i < numChunks; ++i, o += size)
    //        chunks[i] = str.substr(o, size)
    //    return chunks
    //}
};

window.imageselector = new ImageSelector();