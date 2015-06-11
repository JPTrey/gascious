function getRandomColors(amount) {
    var colors = [];
    switch (myData.length) {
        case 12: colors.push("#E58C24")
        case 11: colors.push("#DEE524")
        case 10: colors.push("#A9A8D4")
        case 9: colors.push("#A8D4A9")
        case 8: colors.push("#D4A9A8")
        case 7: colors.push("#F4ADD8")
        case 6: colors.push("#841590")
        case 5: colors.push("#CFBD1E")
        case 4: colors.push("#CF641E")
        case 3: colors.push("#B33D2F")
        case 2: colors.push("#2FA5B3")
        default: colors.push("#CCFF99")
            break;
    }
    return colors;
}

function buildPieChart(id, title, myData) {
    var colors = getRandomColors(myData.length);
    var myChart = new JSChart(id, "pie");
    myChart.setDataArray(myData);
    myChart.colorizePie(colors);
    myChart.setTitle(title);
    myChart.setTitleColor('#8E8E8E');
    myChart.setTitleFontSize(25);
    myChart.setTextPaddingTop(30);
    myChart.setSize(616, 321);
    myChart.setPiePosition(308, 170);
    myChart.setPieRadius(85);
    myChart.setPieUnitsColor('#555');
    myChart.setBackgroundImage('chart_bg.jpg');
    myChart.draw();
}

function buildBarChart(id, title, myData) {
    var colors = getRandomColors(myData.length);
    var myChart = new JSChart(id, 'bar');
    myChart.setDataArray(myData);
    myChart.colorizeBars(colors);
    myChart.setTitle(title);
    myChart.setTitleColor('#8E8E8E');
    myChart.setAxisNameX('');
    myChart.setAxisNameY('%');
    myChart.setAxisColor('#C4C4C4');
    myChart.setAxisNameFontSize(16);
    myChart.setAxisNameColor('#999');
    myChart.setAxisValuesColor('#7E7E7E');
    myChart.setBarValuesColor('#7E7E7E');
    myChart.setAxisPaddingTop(60);
    myChart.setAxisPaddingRight(140);
    myChart.setAxisPaddingLeft(150);
    myChart.setAxisPaddingBottom(40);
    myChart.setTextPaddingLeft(105);
    myChart.setTitleFontSize(25);
    myChart.setBarBorderWidth(1);
    myChart.setBarBorderColor('#C4C4C4');
    myChart.setBarSpacingRatio(50);
    myChart.setGrid(false);
    myChart.setSize(616, 321);
    myChart.setBackgroundImage('chart_bg.jpg');
    myChart.draw();
}