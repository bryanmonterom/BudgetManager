function initTransactionsForm(url) {
    $(function () {
        $("#IdTransactionType").change(async function () {
            const selectedValue = $(this).val();
            const response = await fetch(url, {
                method: 'POST',
                body: selectedValue,
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            const json = await response.json();
            const options = json.map(a => `<option value=${a.value}>${a.text}</option>`);
            $("#IdCategory").html(options);

        })
    })
}