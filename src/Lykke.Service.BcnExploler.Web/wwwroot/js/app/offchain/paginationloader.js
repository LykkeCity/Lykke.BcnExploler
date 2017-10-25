$(function () {
    var selectors = {
        offchainPage: '.js-offchain-page',
        loader: '#js-offchain-page-loader',
        showMoreBtn: '.js-load-offchain-page',
        mixedTransaction: '.js-transaction-details, .js-offchain-transaction-details',
        loadedTrannsactionCount: '.js-offchain-loaded-transaction-count',
        offchainPageList:'.js-offchain-page-list'
    };


    (function() {
        var loadOffchainPage = function ($page) {
            if ($page.length === 0) {
                return;
            }

            var loadUrl = $page.data('load-url');
            var loadedClass = "js-offchain-loaded";

            if (!$page.hasClass(loadedClass)) {
                var $pageContainer = $page.parents(selectors.offchainPageList);
                $(selectors.loader).removeClass('hidden');
                $.ajax(loadUrl).success(function(resp) {
                    $(selectors.loader).addClass('hidden');
                    $page.html(resp)
                    $page.removeClass('hidden');
                    $page.next().removeClass('hidden'); // show Load more btn on next page"

                    $page.trigger('transactions-loaded');
                    $page.addClass(loadedClass);
                    $(selectors.loadedTrannsactionCount).html($(selectors.mixedTransaction, $pageContainer).length);
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