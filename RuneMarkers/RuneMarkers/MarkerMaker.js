var c = document.getElementById('myCanvas');
var ctx = c.getContext("2d");
ctx.fillStyle = "white";
ctx.fillRect(0,0,300,300);

var invert = true;

var runeArray = new Array(24);

var algiz = new Image();
algiz.src = '/Runes/algiz.png';
runeArray[0] = { img: algiz, name: 'algiz' };

var ansuz = new Image();
ansuz.src = '/Runes/ansuz.png';
runeArray[1] = { img: ansuz, name: 'ansuz' };

var berkanan = new Image();
berkanan.src = '/Runes/berkanan.png';
runeArray[2] = { img: berkanan, name: 'berkanan' };

var dagaz = new Image();
dagaz.src = '/Runes/dagaz.png';
runeArray[3] = { img: dagaz, name: 'dagaz' };

var ehwaz = new Image();
ehwaz.src = '/Runes/ehwaz.png';
runeArray[4] = { img: ehwaz, name: 'ehwaz' };

var fehu = new Image();
fehu.src = '/Runes/fehu.png';
runeArray[5] = { img: fehu, name: 'fehu' };

var gebo = new Image();
gebo.src = '/Runes/gebo.png';
runeArray[6] = { img: gebo, name: 'gebo' };

var haglaz = new Image();
haglaz.src = '/Runes/haglaz.png';
runeArray[7] = { img: haglaz, name: 'haglaz' };

var ingwaz = new Image();
ingwaz.src = '/Runes/ingwaz.png';
runeArray[8] = { img: ingwaz, name: 'ingwaz' };

var isaz = new Image();
isaz.src = '/Runes/isaz.png';
runeArray[9] = { img: isaz, name: 'isaz' };

var iwaz = new Image();
iwaz.src = '/Runes/iwaz.png';
runeArray[10] = { img: iwaz, name: 'iwaz' };

var jeran = new Image();
jeran.src = '/Runes/jeran.png';
runeArray[11] = { img: jeran, name: 'jeran' };

var kauna = new Image();
kauna.src = '/Runes/kauna.png';
runeArray[12] = { img: kauna, name: 'kauna' };

var laukaz = new Image();
laukaz.src = '/Runes/laukaz.png';
runeArray[13] = { img: laukaz, name: 'laukaz' };

var mannaz = new Image();
mannaz.src = '/Runes/mannaz.png';
runeArray[14] = { img: mannaz, name: 'mannaz' };

var naudiz = new Image();
naudiz.src = '/Runes/naudiz.png';
runeArray[15] = { img: naudiz, name: 'naudiz' };

var othalan = new Image();
othalan.src = '/Runes/othalan.png';
runeArray[16] = { img: othalan, name: 'othalan' };

var pertho = new Image();
pertho.src = '/Runes/pertho.png';
runeArray[17] = { img: pertho, name: 'pertho' };

var raido = new Image();
raido.src = '/Runes/raido.png';
runeArray[18] = { img: raido, name: 'raido' };

var sowilo = new Image();
sowilo.src = '/Runes/sowilo.png';
runeArray[19] = { img: sowilo, name: 'sowilo' };

var thurisaz = new Image();
thurisaz.src = '/Runes/thurisaz.png';
runeArray[20] = { img: thurisaz, name: 'thurisaz' };

var tiwaz = new Image();
tiwaz.src = '/Runes/tiwaz.png';
runeArray[21] = { img: tiwaz, name: 'tiwaz' };

var uruz = new Image();
uruz.src = '/Runes/uruz.png';
runeArray[22] = { img: uruz, name: 'uruz' };

var wunjo = new Image();
wunjo.src = '/Runes/wunjo.png';
runeArray[23] = { img: wunjo, name: 'wunjo' };


/*
Shuffles array in place.
@param {Array} a items An array containing the items.
*/
function shuffle(a) {
    var j, x, i;
    for (i = a.length - 1; i > 0; i--) {
        j = Math.floor(Math.random() * (i + 1));
        x = a[i];
        a[i] = a[j];
        a[j] = x;
    }
}

function downloadURI(uri, name) {
  var link = document.createElement("a");
  link.download = name;
  link.href = uri;
  document.body.appendChild(link);
  link.click();
  document.body.removeChild(link);
  delete link;
}
function create(r, x) {
    var centerRune = runeArray[r].img;

    // all Runes are 150 x 200 px
    var cRuneWidth = centerRune.width || centerRune.naturalWidth;
    var cRuneHeight = centerRune.height || centerRune.naturalHeight;
    var smallWidth = cRuneWidth / 4;
    var smallHeight = cRuneHeight / 4;
    var cpy = runeArray.slice();
    shuffle(cpy);
    ctx.drawImage(centerRune, c.width / 2 - cRuneWidth / 2, c.height / 2 - cRuneHeight / 2); //middle
    for (var i = 0; i < 8; i++) {
        ctx.drawImage(cpy[i].img, i * smallWidth, 0, smallWidth, smallHeight);
    }
    for (var i = 0; i < 8; i++) {
        ctx.drawImage(cpy[i + 8].img, i * smallWidth, c.height - smallHeight, smallWidth, smallHeight);
    }
    for (var i = 0; i < 4; i++) {
        ctx.drawImage(cpy[i + 16].img, 0, (i + 1) * smallHeight, smallWidth, smallHeight);
    }
    for (var i = 0; i < 4; i++) {
        ctx.drawImage(cpy[i + 20].img, c.width - smallWidth, (i + 1) * smallHeight, smallWidth, smallHeight);
    }

    ctx.beginPath();
    ctx.lineWidth = "3";
    ctx.rect(smallHeight, smallHeight, cRuneHeight, cRuneHeight);
    ctx.stroke();

    // ctx.beginPath();
    // ctx.lineWidth="6";
    // ctx.rect(0,0,c.width,c.height);
    // ctx.stroke();

    if (invert) {
        // invert center
        var imageData = ctx.getImageData(smallHeight, smallHeight, cRuneHeight, cRuneHeight);
        var data = imageData.data;

        for (var i = 0; i < data.length; i += 4) {
            // red
            data[i] = 255 - data[i];
            // green
            data[i + 1] = 255 - data[i + 1];
            // blue
            data[i + 2] = 255 - data[i + 2];
        }

        // overwrite original center
        ctx.putImageData(imageData, smallHeight, smallHeight);

        //invert all
        imageData = ctx.getImageData(0, 0, c.width, c.height);
        data = imageData.data;
        for (var i = 0; i < data.length; i += 4) {
            // red
            data[i] = 255 - data[i];
            // green
            data[i + 1] = 255 - data[i + 1];
            // blue
            data[i + 2] = 255 - data[i + 2];
        }

        // overwrite original image
        ctx.putImageData(imageData, 0, 0);
        var url = c.toDataURL("image/jpg");
        downloadURI(url, runeArray[r].name + x + ".jpg");
    }
}

window.onload = function () {
    var timeout = 100;
    setTimeout(function () {
        for (var r = 0; r < runeArray.length; r++) {
            for (var x = 0; x < 4; x++) {
                setTimeout(create, timeout * x + timeout*4*r, r, x);
            }
        }
            
    },
    1000);

};
