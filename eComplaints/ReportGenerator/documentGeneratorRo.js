﻿var jsreport = require('jsreport-core')();

module.exports = function (callback, data) {
    jsreport.init().then(function () {
        return jsreport.render({
            template: {
                content: '<html><head><style>.body {font-family: "Calibri";font-size:  large;margin: auto;}.logo {float: left;}.row {padding-left: 10px;}.identity {float: right;padding-right: 20px;font-size: 22px;}.table1{margin-top: 20px;}.table1 table, th, td {padding: 5px;padding-left: 2px;border-collapse: collapse;}.table1 table {margin-left: auto;margin-right: auto;width: 100%;}.table1 table .color td{background: #b5e1f2;}.table1 table tr .input  {padding-left: 10px;}.table1 table .heading {background: #a8abaf;text-align: center;font-size: 18px;border: 1px  solid black;}.table1 table tr .input {text-align: center;border-right: 1px solid black;border-bottom: 1px solid black;}label {font-weight: bold;}.table1 table tr .lbl{border-left: 1px solid black;border-bottom: 1px solid black;font-weight: bold;}.table2{margin-top: 20px;}.table2 table, th, td {padding: 5px;padding-left: 2px;border-collapse: collapse;}.table2 table {margin-left: auto;margin-right: auto;width: 100%;}.table2 table .color td{background: #b5e1f2;}.table2 table tr .input  {padding-left: 10px;}.table2 table .heading {background: #a8abaf;text-align: center;font-size: 18px;border: 1px  solid black;}.table2 table tr .input {text-align: center;border-right: 1px solid black;border-bottom: 1px solid black;}.table2 table tr .lbl{border-left: 1px solid black;border-bottom: 1px solid black;font-weight: bold;}.table3 table {empty-cells: show;}.table3{margin-top: 20px;}.table3 table, th, td {padding: 5px;padding-left: 2px;border-collapse: collapse;}.table3 table {margin-left: auto;margin-right: auto;width: 100%;margin-top:20px;}.table3 table .color td{background: #b5e1f2;}.table3 table tr .input  {padding-left: 10px;}.table3 table .heading {background: #a8abaf;text-align: center;font-size: 18px;border: 1px  solid black;}.table3 table tr .input {text-align: center;border-right: 1px solid black;border-bottom: 1px solid black;}.table3 table tr .lbl{border-left: 1px solid black;border-bottom: 1px solid black;font-weight: bold;}.table3 table tr td, th {border: 1px solid black;height: 50px;}</style></head><body><div class="row"><div class="logo"><img src="C:\\Users\\puchianu.m\\documents\\visual studio 2017\\Projects\\testPDF\\testPDF\\Procter_and_Gamble_Logo.svg.png"/> URLATI Plant, Romania</div><div class="identity">RECLAMATIE MATERIAL </br>NR: {{:identificationNumber}}</br>DATA: {{:dateHour}}</div></div></br></br></br></br><div class = "row table1"><table><tr><td colspan="6" class="heading">Pasul 1 (completat de Originator)</td></tr><tr><td class="lbl">Originator</td><td class="input" colspan="2">{{:originator}}</td><td class="lbl">Data/Ora</td><td class="input" colspan="2">{{:dateHour}}</td></tr><tr class="color"><td class="lbl">Coordonator Linie</td><td class="input">{{:lineCoordinator}}</td><td class="lbl">Linie</td><td class="input">{{:area}}</td><td class="lbl">Echipament</td><td class="input">{{:equipment}}</td></tr><tr ><td class="lbl">GCAS</td><td class="input" colspan="2">{{:gcas}}</td><td class="lbl">BatchSAP</td><td class="input" colspan="2">{{:batchSap}}</td></tr><tr class="color"><td class="lbl">VendorBatch</td><td class="input" colspan="2">{{:vendorBatch}}</td><td class="lbl">PO</td><td class="input" colspan="2">{{:po}}</td></tr><tr><td class="lbl">DownTime</td><td class="input">{{:downtime}}</td><td class="lbl">Nr Opriri</td><td class="input">{{:numberOfStops}}</td><td class="lbl">Mostra</td><td class="input">{{:hasSample}}</td></tr><tr class="color"><td class="lbl">BlockedBatch</td><td class="input">{{:blockedBatch}}</td><td class="lbl">BatchNO</td><td class="input">{{:batchNo}}</td><td class="lbl">Cantitate</td><td class="input">{{:quantity}}</td></tr><tr><td class="lbl">Tip material</td><td class="input" colspan="5">{{:phenomenaCategory}}</td></tr><tr class="color"><td class="lbl">Descriere fenomen</td><td class="input" colspan="5">{{:problem}}</td></tr></table></div><div class="row table2"><table><tr><td class = "heading" colspan = "6">Pasul 2 (completat de Investigator)</td></tr><tr><td class="lbl">Reclamatie relevanta: </td><td class="input" colspan="5">{{:isRelevant}}</td></tr><tr class="color"><td class="lbl">Verificati daca stocul este afetat si blocheaza BATCH:</td><td class="input" colspan="5">{{:batch}}</td></tr><tr><td colspan="6" class = "lbl" style="border-right: 1px solid black; border-bottom: none">Detalii investigatie</td></tr><tr class="color investigation" style="border-left: 1px solid black"><td rowspan="3" colspan="6" class="input">{{:phenomenaDescription}}</td></tr></table></div><div class="row" style="margin-bottom:20px"><div style="margin-top:10px; float: left"><h4>Eticheta palet</h4><img src="{{:etiqueteImagePath}}" height="300" width="400"/></div><div style="margin-top:10px; float:right; text-align:left"><h4>Poza problema</h4><img src="{{:imagePath}}" height="300" width="400"/></div></div><div class="row table3"><table><tr><td class="heading" colspan="4">Pasul 3 (completat de Supplier)</td></tr><tr><th>Action</th><th>Who</th><th>When</th><th>Status</th></tr><tr class="color"><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr><tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr><tr class="color"><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr><tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr></table></div></body></html>',
                engine: 'jsrender',
                recipe: 'phantom-pdf'
            },
            data: data
        }).then(function (resp) {
            callback(/* error */ null, resp.content.toJSON().data);
        });
    }).catch(function (e) {
        callback(/* error */ e, null);
    });
}