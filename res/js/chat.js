var hideTimer = null;

$('ul').hide();
$('ul li').click(function ()
{
    var input = $("input");
    if (!input.is(":visible"))
        return;

    $('li').addClass('active');
    if (!$("messages").find("[data-name='" + $(this).data("name") + "']").hasClass('selected'))
    {
        $('ul li').removeClass('selected');
        $('messages').removeClass('selected active');

        $(this).addClass('selected');
        $('messages[data-name="' + $(this).data("name") + '"]').addClass('selected active');        
    }
});

$(document).keypress(function (e)
{
    var input = $("input");
    if (input.is(":visible")) onKeyUp(e);
});

function activateUI()
{
    $('ul').fadeIn();
    $('messages.selected').addClass('active');
}

function deactivateUI()
{
    $('ul').fadeOut();
    $('messages.selected').removeClass('active');
}

function setFocus(focus)
{
    var mainInput = $("input");
    if (focus)
    {
        clearTimeout(hideTimer);
        activateUI();
        mainInput.show();
        mainInput.val("");
        mainInput.focus();
    }
    else
    {
        mainInput.fadeOut();
        mainInput.val("");
        clearTimeout(hideTimer);
        hideTimer = setTimeout(function () { deactivateUI(); }, 5000);
    }
}

function addMessage(msg)
{
    var child = $("<msg>" + formatMsg(msg) + "</msg>");
    child.hide();
    $("messages.selected").append(child);
    child.fadeIn();

    updateScroll();
}

function formatMsg(input)
{
    var start = '<span style="color: white;">';
    var output = start;

    var pass1 = input.replace("~b~", '</span><span style="color: #07d7ff;">');
    var pass2 = pass1.replace("~g~", '</span><span style="color: #15c39a;">');
    var pass3 = pass2.replace("~r~", '</span><span style="color: #f23452;">');
    var pass4 = pass3.replace("~p~", '</span><span style="color: #ee1289;">');
    var pass5 = pass4.replace("~y~", '</span><span style="color: #faeaac;">');
    var pass6 = pass5.replace("~o~", '</span><span style="color: #ffb733;">');
    var pass7 = pass6.replace("~s~", '</span><span style="color: #bad3e1;">');
    var pass8 = pass7.replace("~w~", '</span><span style="color: #fff;">');

    return output + pass8 + "</span>";
}

function updateScroll()
{
    var body = $("messages");
    if (body.scrollTop() >= body[0].scrollHeight - 400)
    {
        body.scrollTop(body[0].scrollHeight);
    }
}

function onKeyUp(event)
{
    if (event.keyCode == 13)
    {        
        var m = $("input").val();
        try
        {
            resourceCall("commitMessage", (m ? m + "" : " "));
        }
        catch (err)
        {
            $("body").text(err);
        }
        setFocus(false);
    }
}