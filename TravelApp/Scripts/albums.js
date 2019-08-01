$(document).ready(function () {
    //   $(function () {
    getAlbums();
    // });
});

function getAlbums() {
    var url = "/Albums/GetAlbums";
    $.get(url, null, function (data) {
        displayAlbums(data);
    });
}

function createAlbum() {
    var albumName = prompt("Enter Album Name", "");
    if (albumName != null) {
        var url = "/Albums/CreateAlbum";
        var data = { name: albumName };

        $.post(url, data, function (result) {
            var albums = [];
            albums.push(result);
            displayAlbums(albums);
        });
    }
}

function displayAlbums(array) {
    if (array.length == 0) {
        var message = document.createElement("P");
        message.innerText = "You do not have any album. Start creating."
        document.getElementById('albums').appendChild(message);
    }
    for (var i = 0; i < array.length; i++) {
        var linkElement = document.createElement('a');
        //linkElement.href = '@Url.Action("Images/","Image")' + '?containername=' + array[i].ContainerName + '&albumname=' + array[i].AlbumName + '&lastimageindex=' + array[i].LastImageIndex;
        linkElement.href = '/Image/Images' + '?containername=' + array[i].ContainerName + '&albumname=' + array[i].AlbumName + '&lastimageindex=' + array[i].LastImageIndex;
        var img = document.createElement('img');
        img.src = "..\\Images\\albumcover.JPG";
        img.height = "150";
        img.width = "150";

        linkElement.appendChild(img);
        document.getElementById('albums').appendChild(linkElement);
        var albumname = document.createElement("P");
        albumname.innerText = array[i].AlbumName;
        document.getElementById('albums').appendChild(albumname);
    }
    $('#albums').find('img').css({ 'cursor': 'pointer' });
    $('#albums').find('p').css({ 'cursor': 'pointer' });

};