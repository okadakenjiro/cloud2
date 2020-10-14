// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


"use strict";

var clock = {

    clocktime: {},

  dots: document.querySelectorAll('#lcd-clock .dots'),
    
  dotsState: false,
    
  updateClock: function (){

        var time = new Date();
        clock.clocktime.hour   = time.getHours();
        clock.clocktime.minute = time.getMinutes();
        clock.clocktime.second = time.getSeconds();

        for (var timeUnit in clock.clocktime) {
            // convert all to values to string,
            // pad single values, ie 8 to 08
            // split the values into an array of single characters
            clock.clocktime[timeUnit] = clock.clocktime[timeUnit].toString();
            if (clock.clocktime[timeUnit].length == 1) {
                clock.clocktime[timeUnit] = '0'+clock.clocktime[timeUnit];
            }
            clock.clocktime[timeUnit] = clock.clocktime[timeUnit].split('');

            // update each digit for this time unit
            for (var i=0; i<2; i++) {
                var selector = '#lcd-clock .'+timeUnit+'.digit-'+(i+1);
                var className = 'number-is-'+clock.clocktime[timeUnit][i];
                // remove any pre-existing classname
                for (var j=0; j<10; j++) {
                    var oldClass = 'number-is-'+j;
                    document.querySelector(selector).classList.remove(oldClass);
                }
                // add the relevant classname to the appropriate clock digit
                document.querySelector(selector).classList.add(className);
            }

        }

        clock.toggleDots();
    },

    toggleDots: function(){

        var num_dots = clock.dots.length;

        for (var i=0; i < num_dots; i++) {
            if (clock.dotsState === false) {
                clock.dots[i].classList.add('lcd-element-active');
                continue;
            } else {
                clock.dots[i].classList.remove('lcd-element-active');
            }
        }

        clock.dotsState = !clock.dotsState;

    },

    init: function(){

        clock.toggleDots();
        clock.updateClock();
        // update every half second to make dots flash at that rate :)
        setInterval(clock.updateClock, 500);

    }

};



clock.init();




