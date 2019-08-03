// Globals
// We keep this row element as global so that creating new
// albums do not spoil the gallery alignment
var row = document.createElement("div");
var numberOfAlbumsPerRow = 4;

$(document).ready(function () {
    getAlbums();
});

function getAlbums() {
    var url = "/Albums/GetAlbums";
    $.get(url, null, function (data) {
        displayAlbums(data);
    });
};

function createAlbum() {
    var albumName = prompt("Enter Album Name", "");
    if (albumName !== null) {
        var url = "/Albums/CreateAlbum";
        var data = { name: albumName };

        $.post(url, data, function (result) {
            var albums = [];
            albums.push(result);
            displayAlbums(albums);
        });
    }
};

function displayAlbums(array) {
    if (array.length === 0) {
        var message = document.createElement("P");
        message.innerText = "You do not have any album. Start creating.";
        document.getElementById('albums').appendChild(message);
        return;
    }

    // TODO: Use more css wherever possible
    row.className = "album-row";
    for (var i = 0; i < array.length; i++) {

        var linkElement = document.createElement('a');
        linkElement.className = "album-cell";
        linkElement.href = '/Image/Images' + '?containername=' + array[i].ContainerName + '&albumname=' + array[i].AlbumName + '&lastimageindex=' + array[i].LastImageIndex;
        linkElement.style.display = "inline-block";
        var img = document.createElement('img');
        img.src = "..\\Images\\albumcover.JPG";
        img.height = "200";
        img.width = "200";

        var albumname = document.createElement("p");
        albumname.innerText = array[i].AlbumName;
        albumname.style.display = "inline-block";
        albumname.style.width = "200";
        linkElement.style.textAlign = "center";
        
        linkElement.appendChild(img);
        var linebreak = document.createElement('br');
        linkElement.appendChild(linebreak);
        linkElement.appendChild(albumname);
        row.appendChild(linkElement);
        if ((i + 1) % (numberOfAlbumsPerRow) === 0 && i !== 0) {
            document.getElementById('albums').appendChild(row);
            row = document.createElement("div"); 
            row.className = "album-row";
        }
    }
    document.getElementById('albums').appendChild(row);
    $('#albums').find('img').css({ 'cursor': 'pointer' });
    $('#albums').find('p').css({ 'cursor': 'pointer' });

};