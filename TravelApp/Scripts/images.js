var getParams = function (url) {
	var params = {};
	var parser = document.createElement('a');
	parser.href = url;
	var query = parser.search.substring(1);
	var vars = query.split('&');
	for (var i = 0; i < vars.length; i++) {
		var pair = vars[i].split('=');
		params[pair[0]] = decodeURIComponent(pair[1]);
	}
	return params;
}

function OnDeleteItemClicked(filepath) {
    var params = getParams(window.location.href);
    var containername = params['containername'];

    //var url = "/Image/Delete1" + "?containername=" + containername + "&filepath=" + filepath;
    //$.get(url, null, function (result) {
    //        alert(result);
    //    });

    $.ajax({
                type: "GET",
                url: '/Image/Delete' + '?containername=' + containername + '&filepath=' + filepath,
                contentType: "application/json; charset=utf-8",
                success: function (result) {
                   alert(result);
                   document.location.reload();
                },
                error: function (err) {
                   alert(err.statusText);
                }
            });
}

$(document).ready(function(){
    $('#btnUpload').click(function () {

        // Checking whether FormData is available in browser
        if (window.FormData !== undefined) {

            var fileUpload = $("#FileUpload1").get(0);
            var files = fileUpload.files;

            // Create FormData object
            var fileData = new FormData();

            // Looping over all files and add it to FormData object
            for (var i = 0; i < files.length; i++) {
                fileData.append(files[i].name, files[i]);
            }

            // Adding one more key to FormData object
            var params = getParams(window.location.href);
            fileData.append('containername', params['containername']);
            fileData.append('lastimageindex', params['lastimageindex']);
            fileData.append('albumname', params['albumname']);

            $.ajax({
                url: '/Image/Upload',
                type: "POST",
                contentType: false, // Not to set any content header
                processData: false, // Not to process data
                data: fileData,
                success: function (result) {
                   alert(result);
                   //GetImagesImageEndpoint(params['containername'], params['albumname']);
                    document.location.reload();
                },
                error: function (err) {
                   alert(err.statusText);
                   document.location.reload();
                }
            });
        } else {
            alert("FormData is not supported.");
        }
    });
});