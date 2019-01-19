let city = '';
let shown = 25;
let categoryId = '0';
let sortBy = 'date';
let pageNumber = '1';
let pageSize = '25';
let isBusy = false;

function showEvents() {
   
    $.ajax({
        url: "/PublicEvents/ShowEvents",
        type: 'GET',
        data: {
            sortBy: sortBy, categoryId: categoryId, date: $(".datepickerInput").val(), city: city
        },
        success: function (result) {
            $("#events").html(result);
            $(".errorLabel").html("");
            isBusy = false;
        },
        error: function () {
        },
        complete: function () {    
            $('#loading').hide();
            countdown();                  
        }
    }); 
}

function countdown() {
    $('.events').find('.event').each(function () {
        var date = $(this).children()[1];
        var id = $(this).find("#eventId").val();
        var name = $(this).find(".event-name").children()[1].innerHTML;
        var units = $(this).find('.event-timer').children()[0];
        var event = $(this);
        var countDownDate = date.value;
        var now = new Date().getTime() - (new Date().getTimezoneOffset() * 60 * 1000);
        var distance = countDownDate - now;
        var days = Math.floor(distance / (1000 * 60 * 60 * 24));
        var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);
        var interval = 1000;       
        $(this).click(function () {         
            window.location.assign(window.location.protocol + '//' + window.location.host + "/wydarzenie/" + id + "/" + name);       
        });
        $(".shop-list").click(function (evnt) {
            evnt.stopPropagation();
        });
        $(this).find(".follow-button").click(function (evnt) {
            evnt.stopPropagation();
            $.ajax({
                url: "/UserPanel/FollowEvent/",
                type: 'POST',
                data: { eventId: id },
                success: function (result) {
                    event.find(".follow-button").hide();
                    event.find(".unfollow-button").show(0, function () {
                        $(this).find("div").html(result);
                    });
                },
                error: function () {
                },
                complete: function () {

                }
            });

        });
        $(this).find(".unfollow-button").click(function (evnt) {
            evnt.stopPropagation();
            $.ajax({
                url: "/UserPanel/UnfollowEvent/",
                type: 'POST',
                data: { eventId: id },
                success: function (result) {
                    event.find(".unfollow-button").hide();
                    event.find(".follow-button").show(0, function () {
                        $(this).find("div").html(result);
                    });
                },
                error: function () {
                },
                complete: function () {
                }
            });
        });
           
        setInterval(function () {       
        
            now = new Date().getTime() - (new Date().getTimezoneOffset() * 60 * 1000);        
            distance = countDownDate - now;
            days = Math.floor(distance / (1000 * 60 * 60 * 24));
            hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            seconds = Math.floor((distance % (1000 * 60)) / 1000);
            var dateNow = new Date();
     
            if (days === -1 && (hours + dateNow.getHours()) >= -1) {              
                units.style.color = "#E4572E";
                units.children[0].innerHTML = "<d>DZISIAJ</d>";
                units.children[1].innerHTML = "-";
                units.children[2].innerHTML = "-";            
            }
            else if (days <= -1) {
                units.style.color = "white";
                units.children[0].innerHTML = "<d>WCZORAJ</d>";
            }
            else if ((days === 0) && ((hours + dateNow.getHours()) < 23) && ((hours + dateNow.getHours()) > -1)) {
                if (hours < 0) {
                    units.style.color = "#E4572E";
                    units.children[0].innerHTML = "<d>DZISIAJ</d>";
                    units.children[1].innerHTML = "-";
                    units.children[2].innerHTML = "-";           
                }
                else {
                units.style.color = "#E4572E";
                units.children[0].innerHTML = "<d>DZISIAJ</d>";      
                units.children[1].innerHTML = hours;
                units.children[2].innerHTML = minutes;   
                }
            }
            else if (days === 0) {           
                units.children[0].innerHTML = "<d>JUTRO</d>";
                units.children[1].innerHTML = hours;
                units.children[2].innerHTML = minutes;         
            }
            else if (days === 1 && (hours + dateNow.getHours()) < 23) {              
                units.children[0].innerHTML = "<d>JUTRO</d>";
                units.children[1].innerHTML = hours + 24;
                units.children[2].innerHTML = minutes;        
            }               
            else {
                units.children[0].innerHTML = days;
                units.children[1].innerHTML = hours;
                units.children[2].innerHTML = minutes;             
            }
            interval = 30000; 
        }, interval);
    });
   
}

function getCities() {
    var isCity = false;
    var cities = []; 

    $.getJSON("/PublicEvents/GetCities", function (data) {

        $.each(data, function (i, city) {
            cities.push({ label: city, value: city });
        });
    });
    $('#citiesAutocomplete').autocomplete({
        minLength: 3,
        autoFocus: true,
        source: cities,
        select: function (event, ui) {    
            city = ui.item.value;
            isCity = true;
            $("#events").html("");
            $('#loading').show();
            isBusy = true;
            showEvents();
            $('#citiesAutocomplete').blur();
        }            
    });
   
    $('#citiesAutocomplete').focus(function () {
        if (isCity) {
            showEvents();
            isCity = false;
            city = "";
            $('#citiesAutocomplete').val("");
        }
           
        });
    
}


function datepickerHelper() {
    var calHover = false;
    var myDatepicker = $('.datepicker-here').datepicker({
        onSelect: function (formattedDate, date, inst) {
            showEvents();
        }
    }).data('datepicker');

    $(".datepickerInput").hover(

        function () {
            myDatepicker.show();
        },
        function () {
            setTimeout(function () {
                if (!calHover) {
                    myDatepicker.hide();
                }
            }, 300);
        }
    );

    $(".datepicker").hover(
        function () {
            calHover = true;
        },
        function () {
            calHover = false;
            myDatepicker.hide();
        }
    );
}

function showNextEvents() {
    pageNumber++;
    if ($(".errorLabel").html() !== "BRAK WYDARZEŃ" && !isBusy) {
    $.ajax({
        url: "/PublicEvents/ShowEvents/",
        type: 'GET',
        data: {
            sortBy: sortBy, categoryId: categoryId, date: $(".datepickerInput").val(), city: city, pageNumber: pageNumber, pageSize: pageSize
        },
        success: function (result) {          
                $("#events").append(result);   
        },
        error: function () {
         
        },
        complete: function () {
            $('#loading').hide();
            countdown();
            //$(".event-cover").hover(
            //    function () {
            //        $(this).hide();
            //    }
            //);
            //$(".event").hover(
            //    function () {
            //        $(this).find(".event-description").fadeIn("slow");
            //    },
            //    function () {
            //        $(".event-cover").show();
            //        $(this).find(".event-description").fadeOut("fast");
            //    }
            //);
            //if ($(window).width() <= 640) {
            //    $(".events").addClass("list-events");
            //    $(".events").find(".event").each(function () {
            //        $(this).addClass("list-event");
            //    });
            //}
        }
    });
    } else {
        
    }
 
}

function sort() {
 
    $("#desc").click(function () {
        desc = '_desc';
        $("#notDesc").removeClass('active');
        $(this).toggleClass('active');
        shown = 15;
        showEvents({ sortType: $("#orderType").val() + desc, categoryId: catId, city: city });
    });
    $("#notDesc").click(function () {
        desc = '';
        $("#desc").removeClass('active');
        $(this).toggleClass('active');
        shown = 15;
        showEvents({ sortType: $("#orderType").val() + desc, categoryId: catId, city: city });
    });
   
    $("#orderType").change(function () {
        showEvents({ sortType: $("#orderType").val() + desc, categoryId: catId, city: city });
    });

}

$('#sortByDate').click(function () {
    sortBy = 'date';
    $('#sortByFollows').removeClass('icon-active');
    $(this).addClass('icon-active');
    pageNumber = 1;
    showEvents();
});
$('#sortByFollows').click(function () {
    sortBy = 'follows';
    $('#sortByDate').removeClass('icon-active');
    $(this).addClass('icon-active');
    pageNumber = 1;
    showEvents();
});


//$(".fb-share").hover(function () {
//    $(this).html('<i class="fab fa-facebook-square"><d> UDOSTĘPNIJ</d></i>');
//}, function () {
//    $(this).html('<i class="fab fa-facebook-square"></i>');
//    });