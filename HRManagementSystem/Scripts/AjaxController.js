function BAjaxController() {
    this.AjaxGetAdapter =  function (action, div, data, beforefunc, successfunc, onsuccessfunc, onfailfunc, completefunc) {
        $.ajax({
            cache: false,
            url: action,
            data: data,
            beforeSend: function () {
                if (beforefunc != null) {
                    beforefunc();
                }

            },
            success: function (myData) {
                $(div).empty().append(myData);
                if (successfunc != null) {
                    successfunc();
                }

                if (myData == "Success") {
                    if (onsuccessfunc != null) {
                        onsuccessfunc();
                    }
                }
                if (myData == "Fail") {
                    if (onfailfunc != null) {
                        onfailfunc();
                    }
                }

            },
            complete: function () {
                if (completefunc != null) {
                    completefunc();
                }
            }
        });
    }


    this.AjaxPostAdapter = function (action, div, data, beforefunc, successfunc, onsuccessfunc, onfailfunc, completefunc) {
        $.ajax({
            cache: false,
            type: "POST",
            url: action,
            data: data,
            beforeSend: function () {
                if (beforefunc != null) {
                    beforefunc();
                }

            },
            success: function (myData) {
                $(div).empty().append(myData);
                if (successfunc != null) {
                    successfunc();
                }

                if (myData == "Success") {
                    if (onsuccessfunc != null) {
                        onsuccessfunc();
                    }
                }
                if (myData == "Fail") {
                    if (onfailfunc != null) {
                        onfailfunc();
                    }
                }

            },
            complete: function () {
                if (completefunc != null) {
                    completefunc();
                }
            }
        });
    }

    this.AjaxFormPostAdapter = function (action, form, div, beforefunc, successfunc, onsuccessfunc, onfailfunc, completefunc) {
        $(form).submit(function (e) {
            e.preventDefault();
            $.ajax({
                cache: false,
                type: "POST",
                url: action,
                data: $(this).serialize(),
                beforeSend: function () {
                    if (beforefunc != null) {
                        beforefunc();
                    }
                },
                success: function (myData) {
                    $(div).empty().append(myData);
                    if (successfunc != null) {
                        successfunc();
                    }

                    if (myData == "Success") {
                        if (onsuccessfunc != null) {
                            onsuccessfunc();
                        }
                    }
                    if (myData == "Fail") {
                        if (onfailfunc != null) {
                            onfailfunc();
                        }
                    }
                },
                complete: function () {
                    if (completefunc != null) {
                        completefunc();
                    }
                }
            });

        });


    }

};