(function () {
    // Expose a global initialization function for Blazor interop
    window.initPosCategoryCarousel = function () {
        try {
            if (typeof $ === 'undefined' || typeof $.fn === 'undefined' || typeof $.fn.owlCarousel === 'undefined') {
                console.warn('jQuery or Owl Carousel not available when initPosCategoryCarousel was called.');
                return;
            }

            var $el = $('.pos-category');
            if ($el.length === 0) return;

            // If already initialized, destroy first to allow re-init (idempotent)
            if ($el.hasClass('owl-loaded') || $el.hasClass('owl-loaded')) {
                try {
                    $el.trigger('destroy.owl.carousel');
                    $el.removeClass('owl-loaded owl-hidden');
                    $el.find('.owl-stage-outer').children().unwrap();
                } catch (e) {
                    // ignore if destroy fails
                }
            }

            $el.owlCarousel({
                items: 6,
                loop: false,
                margin: 8,
                nav: true,
                dots: false,
                autoplay: false,
                smartSpeed: 1000,
                navText: ['<i class="fas fa-chevron-left"></i>', '<i class="fas fa-chevron-right"></i>'],
                responsive: {
                    0: { items: 2 },
                    500: { items: 3 },
                    768: { items: 4 },
                    991: { items: 5 },
                    1200: { items: 6 },
                    1401: { items: 6 }
                }
            });
        } catch (err) {
            console.error('initPosCategoryCarousel failed', err);
        }
    };
})();
