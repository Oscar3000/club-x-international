document.addEventListener('DOMContentLoaded', function () {
    let wrapper = document.getElementById('wrapper');
    let topLayer = wrapper.querySelector('.top');
    let handle = wrapper.querySelector('.handle');
    let skew = 0;
    let delta = 0;

    if (wrapper.className.indexOf('skewed') != -1) {
        skew = 965;
    }

    wrapper.addEventListener('mousemove', function (e) {
        delta = (e.clientX - window.innerWidth / 2) * 0.5;
        handle.style.left = e.clientX + delta + 'px';

        topLayer.style.width = e.clientX + skew + delta + 'px';
    });
});

$(function () {
    var x = window.matchMedia("(min-width:1030px)");
    $(window).mouseover(function () {
        if (x.matches) {
            var g = $(".HeaderBgVideo").height();
            //console.log(g);
            $(".overlay-wrapper").height(g);
        } else {
            $(".overlay-wrapper").css("height", "");
        }
    });

    //Ajax Call -Blog edit
    //$('#editSubmit').click(function () {
    //    const BlogId = document.getElementById("editBlogID"),
    //     title = document.getElementById("editTitle"),
    //     blogContent = document.getElementById("editor"),
    //     name = document.getElementById("EditName");

    //    var model = {
    //        BlogID: BlogId.value,
    //        Title:blogContent.value,
    //        Name:name.value
    //    };


    //});

    $("#editSubmit").click(function () {
        if (window.FormData !== undefined) {
            var formData = new FormData($('#myForm').get(0));

            $.ajax({
                url: '/Blog/Edit',
                type: "POST",
                contentType: false, // Not to set any content header  
                processData: false, // Not to process data  
                data: formData,
                success: function (result) {
                    //alert(result);
                    showAlert(result, "ajaxSuccess");
                },
                error: function (err) {
                    showAlert(result, "ajaxError");
                }
            });
        } else {
            alert("This browser doesn't support HTML5 file uploads!");
        }
    });


    $(document).ajaxStart(function () {
        $("#LoadingImg").show();
    });

    $(document).ajaxStop(function () {
        $("#LoadingImg").hide();
    });

    function showAlert(message, classname) {
        //Create a div
        const div = document.createElement('div');
        //Add classname
        div.className = `${classname}`;
        //Add text
        div.appendChild(document.createTextNode(message));
        //Get parent node
        var container = document.querySelector('.container');
        //get sibling node
        var form = document.getElementById('myForm');
        //Append div before form
        container.insertBefore(div, form);

        //Disappear after three seconds
        setTimeout(() => {
            document.querySelector(`${classname}`).remove();
        },3000);
    }

    //$(function () {
    //    alert("All good");
    //});
});