/*
  This plugin will take a table and create an area with fixed column or row headers.
  These headers will scroll in relation to how the table scrolls.

  Created By:   Shawn Grover (http://grover.open2space.com)
  Created On:   30 Dec 2007
*/

(function($) {
    /*
    Possible options:
    height            - the height of the table area (default 98%)
    width             - the width of the table area (default 98%)
    scrollDelay       - number of milliseconds to pause before adjusting the headers (default 10)
    forceFooter       - boolean value.  If true, use the thead as the footer (default true)
    If false, use the tfoot (if available).
    rowHeaders        - the number of columns from the left hand side to use as row headers (default 0)
    */

    /*
    Layout:
    - Starting with a plain table, we want to modify the structure to look something like this:

        ------------------------------------------------------
    | Working Area  DIV                                  |
    |            -----------------------------------------
    |            | Column Header DIV                     |
    |            -----------------------------------------
    | ---------- -----------------------------------------
    | | Row    | | Table Area DIV                        |
    | | Headers| |                                       |
    | |        | |                                       |
    | ---------- -----------------------------------------
    |            -----------------------------------------
    |            | Column Footer DIV                     |
    |            -----------------------------------------
    ------------------------------------------------------

        And the original table would be contained in teh Table Area DIV, with the corresponding thead, tfoot, and X number of columns hidden.
    The table area div would have it's overflow set to AUTO, and the scrolling event would be intercepted to adjsut the offset of the header/footer areas.
    */

    /* **************************************************/
    //    Main Plugin
    /* **************************************************/
    jQuery.fn.fixedTableHeaders = function(options) {
        var opts = $.extend({}, options);
        var me = this;

        //Add div wrapper around working area
        var workarea = "<div id=\"fixedHeaderArea\"></div>";
        $(this).wrap(workarea);
        workarea = $("#fixedHeaderArea");

        //Add div around table (for scrolling purposes
        var tblarea = "<div id=\"fixedHeaderTable\"></div>";
        $(this).wrap(tblarea);
        tblarea = $("#fixedHeaderTable");

        //Add the column header, row header, and footer areas
        var colhead = "<div id=\"fixedHeaderColumns\">&nbsp;</div>";
        var rowhead = "<div id=\"fixedHeaderRows\" style=\"float: left;\">&nbsp;</div>";
        var foothead = "<div id=\"fixedHeaderFooter\">&nbsp;</div>";
        $(tblarea).before(colhead);
        $(tblarea).after(foothead);

        if (opts.rowHeaders) {
            $(tblarea).before(rowhead);
        }

        colhead = $("#fixedHeaderColumns");
        rowhead = $("#fixedHeaderRows");
        foothead = $("#fixedHeaderFooter");


        buildColumnHeaders(this, opts);
        buildRowHeaders(this, opts);
        sizeHeaders(opts, true);


        //apply scrolling event handlers
        $(tblarea).unbind("scroll").scroll(function() {
            if (tblarea[0].scrollTimer) {
                clearTimeout(tblarea[0].scrollTimer);
            }
            tblarea[0].scrollTimer = setTimeout(scrollHeaders, options.scrollDelay || 10);
        });

        //make sure we readjust when the screen is resized
        $(window).resize(function() {
            sizeHeaders(opts, false);
        });

        //Debugging purposes
        $(workarea).css({ border: "0px solid transparent" });
        $(tblarea).css({ border: "0px solid transparent" });
        $(colhead).css({ "background-color": "#f2f2f2" });
        $(rowhead).css({ "background-color": "#e8eef7" });
        $(foothead).css({ "background-color": "#fff" });
    };

    /* **************************************************/
    //    Private Functions
    /* **************************************************/

    //Move the thead section into our column header area
    function buildColumnHeaders(src, options) {
        var srcHead = $(src).children("thead");

        var data = "";
        $(srcHead).children("tr").each(function() {
            data += "<tr>";
            $(this).children("th").each(function() {
                data += "<td><div class=\"headerContent\">" + $(this).html() + "</div></td>";
            });
            data += "</tr>";
        });

        var heads = "<table border='0' cellpadding='0' cellspacing='0'><tbody>" + data + "</tbody></table>";
        $("#fixedHeaderColumns").html(heads);
        srcHead.hide();

        var foots = "<table><tbody>";
        if (options.forceFooter) {
            foots += data + "</tbody></table>";
            $("#fixedHeaderFooter").html(foots);
            $(src).children("tfoot").hide();
        }
        else {
            var srcFoot = $(src).children("tfoot");
            if (srcFoot.length) {
                $(srcFoot).children("tr").each(function() {
                    foots += "<tr>";
                    $(this).children("th").each(function() {
                        foots += "<td><div class=\"headerContent\">" + $(this).html() + "</div></td>";
                    });
                    foots += "</tr>";
                });
                foots += "</tbody></table>";
                $("#fixedHeaderFooter").html(foots);
                srcFoot.hide();
            }
        }
    }

    //move the desired rows to the row header area
    function buildRowHeaders(src, options) {
        if (!options) { return; }
        if (options.rowHeaders && options.rowHeaders > 0) {
            //We are going to grab the first X columns (specified by options.rowHeaders) and use those
            //as our row header values.

            var rows = "<table border='0' cellpadding='0' cellspacing='0'><tbody>";
            $(src).children("tbody").children("tr").each(function() {
                //For each row, we need to get the contents of the first X cells
                rows += "<tr>";
                $(this).children("td:lt(" + options.rowHeaders + ")").each(function() {
                    rows += "<td><div class=\"headerContent\">" + $(this).html() + "</div></td>";
                    $(this).hide();
                });
                rows += "</tr>";
            });
            rows += "</tbody></table>";

            $("#fixedHeaderRows").html(rows);


            //We need to also remove the first X number of columns on the header and header rows
            $("#fixedHeaderColumns td:lt(" + options.rowHeaders + ")").hide();
            $("#fixedHeaderFooter td:lt(" + options.rowHeaders + ")").hide();
        }
    }

    //Adjust the size of the DIVs
    function sizeHeaders(opts, isCreating) {
        var tblarea = $("#fixedHeaderTable");
        var colhead = $("#fixedHeaderColumns");
        var rowHead = $("#fixedHeaderRows");
        var foothead = $("#fixedHeaderFooter");

        //Afegit per redimensionar tota la taula en temps d'execució    
        switch (BrowserDetect.browser) {
            case 'Firefox':
                if (opts.width != "auto")
                    opts.width = document.body.clientWidth - 540;

                if (actualTab == 0) { //Si es el primer calendari, una mida, el segon per la barra horitzontal...
                    opts.height = document.body.clientHeight - 190; //220;
                }
                else {
                    if (ShowCoverage == true) {
                        opts.height = document.body.clientHeight - 300; //320;
                    }
                    else {
                        opts.height = document.body.clientHeight - 270; //290;
                    }

                    //var palette = document.getElementById('divPalettePlan')
                    //if (palette != null)
                    //    palette.style.width = opts.width + 200 + "px";
                }
                break;

            case 'Explorer':
                if (opts.width != "auto")
                    opts.width = document.body.clientWidth - 530;

                if (actualTab == 0) {
                    opts.height = document.body.clientHeight - 220;
                }
                else {
                    if (ShowCoverage == true) {
                        opts.height = document.body.clientHeight - 310;
                    }
                    else {
                        opts.height = document.body.clientHeight - 280;
                    }
                    //var palette = document.getElementById('divPalettePlan')
                    //if (palette != null)
                    //    palette.style.width = opts.width + 200 + "px";
                }
                break;

            default:
                if (opts.width != "auto")
                    opts.width = document.body.clientWidth - 530;

                if (actualTab == 0) {
                    opts.height = document.body.clientHeight - 220 + 54;
                }
                else {
                    opts.height = document.body.clientHeight - 290 + 54;
                }
                //var palette = document.getElementById('divPalettePlan')
                //if (palette != null)
                //    palette.style.width = opts.width + 200 + "px";

                break;
        }
        // Si el Selector esta ocult, dona 200px mes
        if (isSelectorVisible == false) {
            if (opts.width != "auto")
                opts.width = opts.width + 200;
        }

        var widthPlus = 0;
        if (document.getElementById('fixedHeaderTable') != null && document.getElementById('fixedHeaderTable') != null) {
            if (document.getElementById('fixedHeaderTable').scrollHeight > document.getElementById('fixedHeaderTable').clientHeight) {
                widthPlus = 17;
            }
        }

        //These widths and positions need to be set AFTER the headers have been sized        
        //Set the width of the table area
        $(tblarea).css({
            width: (opts.width + widthPlus) || "auto",
            height: 550 || "98%",
            overflow: "auto"
        });

        var horitzontalScrooller = 0;
        if($(tblarea).get(0).scrollHeight > $(tblarea).height() && $(tblarea).get(0).scrollWidth > $(tblarea).width()){
            horitzontalScrooller = 18;
        }

        var horizontalOffset = 0;
        if ($(rowHead).length) { horizontalOffset = $(rowHead).width(); }

        $(colhead).css({
            position: "relative",
            left: horizontalOffset,
            width: $(tblarea).width() - widthPlus /*- 16*/,
            overflow: "hidden"
        });

        if (opts.rowHeaders) {
            $(rowHead).css({
                height: $(tblarea).height() - horitzontalScrooller,
                overflow: "hidden"
            });
        }

        $(foothead).css({
            position: "relative",
            left: horizontalOffset,
            width: $(tblarea).width() - 16,
            overflow: "hidden",
            display: "none"
        });
    }

    //Adjust the position of the headers based on a scroll
    function scrollHeaders() {
        var tblarea = $("#fixedHeaderTable");
        var rowsArea = $('#fixedHeaderRows');
        var x = tblarea[0].scrollLeft;
        var y = tblarea[0].scrollTop;
        
        //VTCUSTOM: Posicionament personalitzat per recarregues
        if (saveScroll) {
            x = scrollGrid_x;
            y = scrollGrid_y;
            saveScroll = false;
        } else {
            scrollGrid_x = x;
            scrollGrid_y = y;
        }

        $("#fixedHeaderColumns")[0].scrollLeft = x;
        $("#fixedHeaderFooter")[0].scrollLeft = x;

        if (rowsArea.length) {
            rowsArea[0].scrollTop = y;
        }
    }

})(jQuery);


