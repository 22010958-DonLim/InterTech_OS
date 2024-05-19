// ==================================================
// Project Name   :  Borne - Agency Portfolio Template
// File           :  JS Base
// Version        :  1.0.0
// Last change    :  Sunday, 06 June 2021
// Author         :  wp_studio
// Author URL     :  http://themeforest.net/user/wp_studio
// ==================================================

(function($) {
  "use strict";

    /*------------------------------------------
        = back to top
    -------------------------------------------*/
    $(window).on("scroll", function() {
        if ($(this).scrollTop() > 200) {
            $('.backtotop:hidden').stop(true, true).fadeIn();
        } else {
            $('.backtotop').stop(true, true).fadeOut();
        }
    })

    $(function() {
        $(".scroll").on('click', function() {
            $("html,body").animate({scrollTop: 0}, "slow");
            return false
        });
    });



    /*------------------------------------------
        = preloader
    -------------------------------------------*/
    $(window).on('load', function(){
        $('#preloader').fadeOut('slow',function(){$(this).remove();});
    });



    /*------------------------------------------
        = sticky header
    -------------------------------------------*/
    $(window).on('scroll', function () {
        if ($(this).scrollTop() > 120) {
            $('.header_section').addClass("sticky")
        } else {
            $('.header_section').removeClass("sticky")
        }
    });



    /*------------------------------------------
        = pointer cursor
    -------------------------------------------*/
    if ($('.pointer-wrap').length) {
        const cursor = cursorDot({})
        cursor.over("a, button", {
            scale: '1.4'
        });
    }



    /*------------------------------------------
        = popup images & videos
    -------------------------------------------*/
    $('.popup_video').magnificPopup({
        type: 'iframe',
        preloader: false,
        removalDelay: 160,
        mainClass: 'mfp-fade',
        fixedContentPos: false
    });

    $('.zoom-gallery').magnificPopup({
        delegate: '.popup_image',
        type: 'image',
        closeOnContentClick: false,
        closeBtnInside: false,
        mainClass: 'mfp-with-zoom mfp-img-mobile',
        gallery: {
            enabled: true
        },
        zoom: {
            enabled: true,
            duration: 300,
            opener: function(element) {
                return element.find('img');
            }
        }
    });



    /*------------------------------------------
        = nice select
    -------------------------------------------*/
    $('select').niceSelect();



    /*------------------------------------------
        = sidebar
    -------------------------------------------*/
    $(document).ready(function () {
        $('.close_btn, .overlay').on('click', function () {
            $('.info_sidebar').removeClass('active');
            $('.overlay').removeClass('active');
        });

        $('.moreinfo_sidebar_btn').on('click', function () {
            $('.info_sidebar').addClass('active');
            $('.overlay').addClass('active');
        });
    });



    /*------------------------------------------
        = main slider
    -------------------------------------------*/
    $('.main_slider').slick({
        dots: true,
        fade: true,
        arrows: true,
        infinite: true,
        autoplay: true,
        slidesToShow: 1,
        autoplaySpeed: 5000,
        prevArrow: ".main_left_arrow",
        nextArrow: ".main_right_arrow"
    });

    $('.main_slider').on('init', function (e, slick) {
        var $firstAnimatingElements = $('div.slider_item:first-child').find('[data-animation]');
        doAnimations($firstAnimatingElements);
    });

    $('.main_slider').on('beforeChange', function (e, slick, currentSlide, nextSlide) {
        var $animatingElements = $('div.slider_item[data-slick-index="' + nextSlide + '"]').find('[data-animation]');
        doAnimations($animatingElements);
    });

    var slideCount = null;

    $('.main_slider').on('init', function (event, slick) {
        slideCount = slick.slideCount;
        setSlideCount();
        setCurrentSlideNumber(slick.currentSlide);
    });

    $('.main_slider').on('beforeChange', function (event, slick, currentSlide, nextSlide) {
        setCurrentSlideNumber(nextSlide);
    });

    function setSlideCount() {
        var $el = $('.slide_count_wrap').find('.total');
            if (slideCount < 10) {
            $el.text('0' + slideCount);
        } else {
            $el.text(slideCount);
        }
    }

    function setCurrentSlideNumber(currentSlide) {
        var $el = $('.slide_count_wrap').find('.current');
        $el.text(currentSlide + 1);
    }

    function doAnimations(elements) {
        var animationEndEvents = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
        elements.each(function () {
            var $this = $(this);
            var $animationDelay = $this.data('delay');
            var $animationType = 'animated ' + $this.data('animation');
            $this.css({
                'animation-delay': $animationDelay,
                '-webkit-animation-delay': $animationDelay
            });
            $this.addClass($animationType).one(animationEndEvents, function() {
                $this.removeClass($animationType);
            });
        });
    }

    var $timer = 6000;
    function progressBar() {
        $(".slick-progress").find("span").removeAttr("style");
        $(".slick-progress").find("span").removeClass("active");
        setTimeout(function() {
            $(".slick-progress").find("span").css("transition-duration", $timer / 1000 + "s").addClass("active");
        }, 100);
    }

    progressBar();

    $('.main_slider').on("beforeChange", function (e, slick) {
        progressBar();
    });



    /*------------------------------------------
        = google map
    -------------------------------------------*/
    if ( $('#mapBox').length ){
        var $lat = $('#mapBox').data('lat');
        var $lon = $('#mapBox').data('lon');
        var $zoom = $('#mapBox').data('zoom');
        var $marker = $('#mapBox').data('marker');
        var $info = $('#mapBox').data('info');
        var $markerLat = $('#mapBox').data('mlat');
        var $markerLon = $('#mapBox').data('mlon');
        var map = new GMaps({
            el: '#mapBox',
            lat: $lat,
            lng: $lon,
            scrollwheel: false,
            scaleControl: true,
            streetViewControl: false,
            panControl: true,
            disableDoubleClickZoom: true,
            mapTypeControl: false,
            zoom: $zoom,
        });
        map.addMarker({
            lat: $markerLat,
            lng: $markerLon,
            icon: $marker,    
            infoWindow: {
                content: $info
            }
        })
    }



    /*------------------------------------------
        = testimonial carousel
    -------------------------------------------*/
    $('.testimonial_carousel').slick({
        speed: 1000,
        autoplay: true,
        infinite: true,
        slidesToShow: 2,
        slidesToScroll: 1,
        pauseOnHover: true,
        autoplaySpeed: 3000,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                }
            },

            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                }
            }
        ]
    });



    /*------------------------------------------
        = team carousel
    -------------------------------------------*/
    $('.team_carousel').slick({
        speed: 1000,
        autoplay: true,
        infinite: true,
        slidesToShow: 2,
        slidesToScroll: 1,
        pauseOnHover: true,
        autoplaySpeed: 3000,
        responsive: [
            {
                breakpoint: 1024,
                settings: {
                    slidesToShow: 3,
                    slidesToScroll: 1,
                }
            },

            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 2,
                    slidesToScroll: 1,
                }
            },

            {
            breakpoint: 381,
                settings: {
                    slidesToShow: 1,
                    slidesToScroll: 1,
                }
            }
        ]
    });



    /*------------------------------------------
        = blog image carousel
    -------------------------------------------*/
    $('.blog_image_carousel').slick({
        speed: 1000,
        dots: false,
        infinite: true,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        pauseOnHover: true,
        autoplaySpeed: 3000,
        prevArrow: ".bic_left_arrow",
        nextArrow: ".bic_right_arrow",
    });



    /*------------------------------------------
        = portfolio filter
    -------------------------------------------*/
    var $element_grid = $(".element_grid").isotope({
        itemSelector: ".element-item",
        layoutMode: "fitRows"
    });
    var filterFns = {
        numberGreaterThan50: function() {
            var number = $(this).find(".number").text();
        },
        ium: function() {
            var name = $(this).find(".name").text();
            return name.match(/ium$/);
        }
    };
    $(".filter-btns-group").on("click", "button", function() {
        var filterValue = $(this).attr("data-filter");
        filterValue = filterFns[filterValue] || filterValue;
        $element_grid.isotope({ filter: filterValue });
    });
    $(".button-group").each(function(i, buttonGroup) {
        var $buttonGroup = $(buttonGroup);
        $buttonGroup.on("click", "button", function() {
            $buttonGroup.find(".active").removeClass("active");
            $(this).addClass("active");
        });
    });



    /*------------------------------------------
        = quantity
    -------------------------------------------*/
    (function() {
        window.inputNumber = function(el) {
            var min = el.attr("min") || false;
            var max = el.attr("max") || false;

            var els = {};

            els.dec = el.prev();
            els.inc = el.next();

            el.each(function() {
                init($(this));
            });

            function init(el) {
                els.dec.on("click", decrement);
                els.inc.on("click", increment);

                function decrement() {
                    var value = el[0].value;
                    value--;
                    if (!min || value >= min) {
                        el[0].value = value;
                    }
                }

                function increment() {
                    var value = el[0].value;
                    value++;
                    if (!max || value <= max) {
                        el[0].value = value++;
                    }
                }
            }
        };
    })();
    inputNumber($(".input_number"));


		
	/**
	* contact form action
	**/

	function validateEmail(email) {
		const re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
		return re.test(String(email).toLowerCase());
	}
	
	function clearForm() {
		const name = $('#contact-form-name');
		const email = $('#contact-form-email');
		const message = $('#contact-form-message');
		
		name.removeClass('error');
		email.removeClass('error');
		message.removeClass('error');
		
		name.val("");
		email.val("");
		message.val("");
	}
	
	$('#contact-form-submit').click(function(e){
		e.preventDefault();
	
		const name = $('#contact-form-name');
		const email = $('#contact-form-email');
		const message = $('#contact-form-message');
		let result = true;
		
		name.removeClass('error');
		email.removeClass('error');
		message.removeClass('error');
		
		if(name.val().trim() === ''){
		   	result = false;
			name.addClass('error');
		}

		if(message.val().trim() === ''){
		   	result = false;
			message.addClass('error');
		}
		
		if(email.val().trim() === '' || !validateEmail(email.val())){
		   	result = false;
			email.addClass('error');
		}
		
		if(!result){
			return;
		}
		
		var formData = {
			name: name.val(),
			email: email.val(),
			message: message.val()
		};

		var jqxhr = $.post( "mail.php", formData, function() {
		  console.log("success");
		})
		.done(function() {
			$('#form-msg').html('Email Send success');
			clearForm();
	  	})
	  	.fail(function() {
			$('#form-msg').html('Email Send error');
	  	});
		
	});
	

    /*------------------------------------------
        = parallax
    -------------------------------------------*/
    if ($('.scene').length > 0 ) {
        $('.scene').parallax({
            scalarX: -10.0,
            scalarY: -10.0,
        }); 
    }

})(jQuery);