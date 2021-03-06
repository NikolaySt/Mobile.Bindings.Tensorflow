var Serialize = function () {
    this.getData = async function (instance, data, getCallback) {
        //data = JSON.parse(json);
        //console.log("Data came. " + data.length);
        //json = JSON.stringify(data);
        await instance.invokeMethodAsync(getCallback, data);
    }
}

window.serialize = new Serialize();