    $(document).ready(function () {
        $("#stdbtn").on('click', function () {

            $.ajax({
                type: "POST",
                url: "/Admin/getStudentByClassJson",
                data: {
                    "classID": $("#ClassSelector").val()
                },
                success: function (response) {

                    $("#stdlist").html(response);
                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });
        });

        $("#getChallan").on('click', function () {
            debugger;
            var req = {
                "rollnumber": $("#stdSelector").val(),
                "month": null,
                "Annual": false,
                "AdmissionFee": false

            };
            $.ajax({
                type: "POST",
                url: "/Admin/getChallanOneStudent",
                data:req,
                success: function (response) {
                    $("#challan").html(response);
                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });
        });
    

        
});

function printContent(el) {
    var restorepage = $('body').html();
    var printcontent = $('#' + el).clone();
    $('body').empty().html(printcontent);
    window.print();
    $('body').html(restorepage);
}