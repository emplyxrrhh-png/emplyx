(function () {
    var namespace = function (name) {
        var namespaces = name.split('.'),
            namespace = window,
            index;
        for (index = 0; index < namespaces.length; index += 1) {
            namespace = namespace[namespaces[index]] = namespace[namespaces[index]] || {};
        }
        return namespace;
    };

    namespace("Robotics.Client.Common");
}());

Robotics.Client.Common.roHtmlColor = function () {
};

Robotics.Client.Common.roHtmlColor.prototype.randomColorWithSeed = function (seedR, seedG, seedB) {
    //var rgb = [this.normalize(seedR, 0, 2359), this.normalize(seedG, 0, 2359), this.normalize(seedB, 0, 1440)];
    //var mixedrgb = [rgb[0] * 255, rgb[1] * 255, rgb[2] * 255].map(function (x) { return Math.round(x / 2.0) })
    //return this.rgb2hexColor({ r: mixedrgb[0], g: mixedrgb[1], b: mixedrgb[2] });

    return '#' + this.dec2hex(parseInt(seedR.toString() + seedG.toString() + seedB.toString(), 10)).substr(0, 6);
};

Robotics.Client.Common.roHtmlColor.prototype.normalize = function (val, max, min) {
    return (val - min) / (max - min);
};

Robotics.Client.Common.roHtmlColor.prototype.randomColor = function () {
    var color;
    color = Math.floor(Math.random() * 0x1000000); // integer between 0x0 and 0xFFFFFF
    color = color.toString(16); // convert to hex
    color = ("000000" + color).slice(-6); // pad with leading zeros
    color = "#" + color; // prepend #
    return color;
};

Robotics.Client.Common.roHtmlColor.prototype.ColorLuminance = function (hex, lum) {
    // validate hex string
    hex = String(hex).replace(/[^0-9a-f]/gi, '');
    if (hex.length < 6) {
        hex = hex[0] + hex[0] + hex[1] + hex[1] + hex[2] + hex[2];
    }
    lum = lum || 0;

    // convert to decimal and change luminosity
    var rgb = "#", c, i;
    for (i = 0; i < 3; i++) {
        c = parseInt(hex.substr(i * 2, 2), 16);
        c = Math.round(Math.min(Math.max(0, c + (c * lum)), 255)).toString(16);
        rgb += ("00" + c).substr(c.length);
    }

    return rgb;
};

Robotics.Client.Common.roHtmlColor.prototype.sbcRip = function (i, d) {
    var l = d.length, RGB = new Object();
    if (l > 9) {
        d = d.split(",");
        if (d.length < 3 || d.length > 4) return null;//ErrorCheck
        RGB[0] = i(d[0].slice(4)), RGB[1] = i(d[1]), RGB[2] = i(d[2]), RGB[3] = d[3] ? parseFloat(d[3]) : -1;
    } else {
        if (l == 8 || l == 6 || l < 4) return null; //ErrorCheck
        if (l < 6) d = "#" + d[1] + d[1] + d[2] + d[2] + d[3] + d[3] + (l > 4 ? d[4] + "" + d[4] : ""); //3 digit
        d = i(d.slice(1), 16), RGB[0] = d >> 16 & 255, RGB[1] = d >> 8 & 255, RGB[2] = d & 255, RGB[3] = l == 9 || l == 5 ? r(((d >> 24 & 255) / 255) * 10000) / 10000 : -1;
    }
    return RGB;
};

Robotics.Client.Common.roHtmlColor.prototype.shadeBlendConvert = function (p, from, to) {
    if (typeof (p) != "number" || p < -1 || p > 1 || typeof (from) != "string" || (from[0] != 'r' && from[0] != '#') || (typeof (to) != "string" && typeof (to) != "undefined")) return null; //ErrorCheck
    var i = parseInt, r = Math.round, h = from.length > 9, h = typeof (to) == "string" ? to.length > 9 ? true : to == "c" ? !h : false : h, b = p < 0, p = b ? p * -1 : p, to = to && to != "c" ? to : b ? "#000000" : "#FFFFFF", f = this.sbcRip(i, from), t = this.sbcRip(i, to);
    if (!f || !t) return null; //ErrorCheck
    if (h) return "rgb(" + r((t[0] - f[0]) * p + f[0]) + "," + r((t[1] - f[1]) * p + f[1]) + "," + r((t[2] - f[2]) * p + f[2]) + (f[3] < 0 && t[3] < 0 ? ")" : "," + (f[3] > -1 && t[3] > -1 ? r(((t[3] - f[3]) * p + f[3]) * 10000) / 10000 : t[3] < 0 ? f[3] : t[3]) + ")");
    else return "#" + (0x100000000 + (f[3] > -1 && t[3] > -1 ? r(((t[3] - f[3]) * p + f[3]) * 255) : t[3] > -1 ? r(t[3] * 255) : f[3] > -1 ? r(f[3] * 255) : 255) * 0x1000000 + r((t[0] - f[0]) * p + f[0]) * 0x10000 + r((t[1] - f[1]) * p + f[1]) * 0x100 + r((t[2] - f[2]) * p + f[2])).toString(16).slice(f[3] > -1 || t[3] > -1 ? 1 : 3);
};

Robotics.Client.Common.roHtmlColor.prototype.invertCssColor = function (color) {
    var rgb = this.invertColor(this.hexColor2rgb(color));
    return this.rgb2hexColor(rgb);

    //return this.shadeBlendConvert(0.5, this.invertSimpleColor(color));
};

Robotics.Client.Common.roHtmlColor.prototype.invertSimpleColor = function (hexTripletColor) {
    var color = hexTripletColor;
    color = color.substring(1);           // remove #
    color = parseInt(color, 16);          // convert to integer
    color = 0xFFFFFF ^ color;             // invert three bytes
    color = color.toString(16);           // convert to hex
    color = ("000000" + color).slice(-6); // pad with leading zeros
    color = "#" + color;                  // prepend #
    return color;
};

Robotics.Client.Common.roHtmlColor.prototype.invertColor = function (rgb) {
    var yuv = this.rgb2yuv(rgb);
    var factor = 90;
    var threshold = 100;
    yuv.y = this.clamp(yuv.y + (yuv.y > threshold ? -factor : factor));
    return this.yuv2rgb(yuv);
};

Robotics.Client.Common.roHtmlColor.prototype.rgb2hexColor = function (rgb) {
    return '#' + this.dec2hex(rgb.r) + this.dec2hex(rgb.g) + this.dec2hex(rgb.b);
};

Robotics.Client.Common.roHtmlColor.prototype.hexColor2rgb = function (color) {
    color = color.substring(1); // remove #
    return {
        r: parseInt(color.substring(0, 2), 16),
        g: parseInt(color.substring(2, 4), 16),
        b: parseInt(color.substring(4, 6), 16)
    };
};

Robotics.Client.Common.roHtmlColor.prototype.rgb2hexColor = function (rgb) {
    return '#' + this.dec2hex(rgb.r) + this.dec2hex(rgb.g) + this.dec2hex(rgb.b);
};

Robotics.Client.Common.roHtmlColor.prototype.dec2hex = function (n) {
    var hex = n.toString(16);
    if (hex.length < 2) {
        return '0' + hex;
    }
    return hex;
};

Robotics.Client.Common.roHtmlColor.prototype.rgb2yuv = function (rgb) {
    var y = this.clamp(rgb.r * 0.29900 + rgb.g * 0.587 + rgb.b * 0.114);
    var u = this.clamp(rgb.r * -0.16874 + rgb.g * -0.33126 + rgb.b * 0.50000 + 128);
    var v = this.clamp(rgb.r * 0.50000 + rgb.g * -0.41869 + rgb.b * -0.08131 + 128);
    return { y: y, u: u, v: v };
};

Robotics.Client.Common.roHtmlColor.prototype.yuv2rgb = function (yuv) {
    var y = yuv.y;
    var u = yuv.u;
    var v = yuv.v;
    var r = this.clamp(y + (v - 128) * 1.40200);
    var g = this.clamp(y + (u - 128) * -0.34414 + (v - 128) * -0.71414);
    var b = this.clamp(y + (u - 128) * 1.77200);
    return { r: r, g: g, b: b };
};

Robotics.Client.Common.roHtmlColor.prototype.clamp = function (n) {
    if (n < 0) { return 0; }
    if (n > 255) { return 255; }
    return Math.floor(n);
};

Robotics.Client.Common.roHtmlColor.prototype.fromWin32Color = function (num) {
    num >>>= 0;
    var b = num & 0xFF;
    var g = (num & 0xFF00) >>> 8;
    var r = (num & 0xFF0000) >>> 16;

    return this.rgb2hexColor({ r: r, g: g, b: b });
};