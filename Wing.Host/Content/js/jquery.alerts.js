(function ($) {
    $.alerts = {
        // defaults
        defaults: {
            itemBorderWidth: 2,
            itemPaddingTop: 2,
            itemPaddingBottom: 2,
            itemMarginTop: 3,
            autoHideInterval: 10000,
            animationSpeed: 6,
            maxVisibleAlerts: 3
        },

        // display next alert 
        displayNextAlert: function (state) {
            if (!state.queue.length)
                return;
            if (state.working)
                setTimeout(function () { $.alerts.displayNextAlert(state); }, state.animationSpeed * 150);
            else if (state.visibleCount < state.maxVisibleAlerts) {
                state.working = true;
                var nextData = state.queue.shift();
                var holder = state.holder;
                var item = $('<div class="jquery-alert-item"><span class="jquery-alert-item-header"><a href="#">fechar</a></span><span class="jquery-alert-item-content">' + nextData.message + '</span></div>');
                state.visibleCount++;
                item.appendTo(holder);
                item.css('padding-top', state.itemPaddingTop + "px");
                item.css('padding-bottom', state.itemPaddingBottom + "px");
                item.css('margin-top', state.itemMarginTop + 'px');
                item.css('border-width', state.itemBorderWidth + 'px');
                holder.animate({ height: holder.height() + item.height() + state.heightDif }, item.height() * state.animationSpeed, function () {
                    if (nextData.autoHide)
                        setTimeout(function () { $.alerts.hideItem(item, state); }, state.autoHideInterval);
                    state.working = false;
                    $.alerts.displayNextAlert(state);
                });
            }
        },

        // hide an alert
        hideItem: function (item, state) {
            if (state.working)
                setTimeout(function () { $.alerts.hideItem(item, state); }, 100);
            else {
                state.working = true;
                item.fadeOut('slow', function () {
                    var holder = state.holder;
                    holder.height(holder.height() - item.height() - state.heightDif);
                    item.remove();
                    if (!holder.find(".jquery-alert-item").length)
                        holder.height(0);
                    state.visibleCount--;
                    state.working = false;
                    $.alerts.displayNextAlert(state);
                });
            }
        }
    };

    $.fn.alertsTarget = function (options) {
        state = $.extend({}, $.alerts.defaults, options, { holder: null, queue: [], working: false, heightDif: 0, visibleCount: 0 });
        return this.each(function () {
            var _this = $(this);
            state.holder = $("<div class='jquery-alerts-holder'></div>");
            state.holder.appendTo(_this);
            state.heightDif = (state.itemBorderWidth * 2) + state.itemPaddingTop + state.itemPaddingBottom + state.itemMarginTop;
            _this.data("alertsState", state);
        });
    };

    $.fn.showAlert = function (message, autoHide) {
        return this.each(function () {
            var _this = $(this);
            var state = _this.data("alertsState");
            autoHide = autoHide == undefined ? true : autoHide == true;

            state.queue[state.queue.length] = { message: message, autoHide: autoHide };
            if (state.queue.length == 1)
                $.alerts.displayNextAlert(state);
        });
    };
})(jQuery);

