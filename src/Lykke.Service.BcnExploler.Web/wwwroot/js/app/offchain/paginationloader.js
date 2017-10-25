﻿$(function () {
    var selectors = {
        offchainPage: '.js-offchain-page',
        loader: '#js-offchain-page-loader',
        showMoreBtn: '.js-load-offchain-page',
        mixedTransaction: '.js-transaction-details, .js-offchain-transaction-details',
        loadedTrannsactionCount: '.js-offchain-loaded-transaction-count'
    };


    (function() {
        var loadOffchainPage = function ($container) {
            if ($container.length === 0) {
                return;
            }

            var loadUrl = $container.data('load-url');
            var loadedClass = "js-offchain-loaded";

            if (!$container.hasClass(loadedClass)) {

                $(selectors.loader).removeClass('hidden');
                $.ajax(loadUrl).success(function(resp) {
                    $(selectors.loader).addClass('hidden');
                    $container.html(resp)
                    $container.removeClass('hidden');
                    $container.next().removeClass('hidden'); // show Load more btn on next page"

                    $container.trigger('transactions-loaded');
                    $container.addClass(loadedClass);
                    $(selectors.loadedTrannsactionCount).html($(selectors.mixedTransaction, $container).length);
                });  
            }
        };

        $('body').on('click', selectors.showMoreBtn, function () {
            var $self = $(this);
            $self.addClass('hidden');

            var $container = $self.parents(selectors.offchainPage);
            loadOffchainPage($container);
        });

        $('body').on('js-tx-tab-toggled address-transaction-list-loaded', function (e) {
            loadOffchainPage($(e.target).find(selectors.offchainPage).first());
        });

        loadOffchainPage($(selectors.offchainPage).first());

    })();

})