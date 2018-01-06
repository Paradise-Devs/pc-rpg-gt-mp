var hideTimer = null;

var messages = [];
var messageidx = -1;

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
        messageidx = -1;
        clearTimeout(hideTimer);
        activateUI();
        mainInput.val("");
        mainInput.show();
        mainInput.focus();
    }
    else
    {
        mainInput.hide();
        mainInput.val("");
        clearTimeout(hideTimer);
        hideTimer = setTimeout(function () { deactivateUI(); }, 5000);
    }
}

function addMessage(msg, chat)
{
    if (!$("messages").find("[data-name='" + chat + "']"))
    {
        console.log("ERROR: Chat tab not found!");
        return;
    }

    activateUI();
    clearTimeout(hideTimer);
    hideTimer = setTimeout(function () { deactivateUI(); }, 5000);

    var child = $("<msg>" + formatMsg(msg) + "</msg>");
    child.hide();
        
    $('messages[data-name="' + chat + '"]').append(child);
    child.fadeIn();

    updateScroll();
}

function clearChat()
{
    $('messages').empty();
}

function formatMsg(input)
{
    var output = '';

    var pass1 = input.replace(/~b~/g, '</span><span style="color: #07d7ff;">');
    var pass2 = pass1.replace(/~g~/g, '</span><span style="color: #31a50d;">');
    var pass3 = pass2.replace(/~r~/g, '</span><span style="color: #f23452;">');
    var pass4 = pass3.replace(/~p~/g, '</span><span style="color: #DA70D6;">');
    var pass5 = pass4.replace(/~y~/g, '</span><span style="color: #ffe801;">');
    var pass6 = pass5.replace(/~o~/g, '</span><span style="color: #ffb733;">');
    var pass7 = pass6.replace(/~s~/g, '</span><span style="color: #bad3e1;">');
    var pass8 = pass7.replace(/~c~/g, '</span><span style="color: #d3d3d3;">');
    var pass9 = pass8.replace(/~w~/g, '</span><span style="color: #fff;">');

    return output + pass9;
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
            if (m) messages.unshift(m);
            var chat = $('messages.selected').data("name");
            resourceCall("commitMessage", (m ? m + "" : " "), chat);
        }
        catch (err)
        {
            $("body").text(err);
        }
        setFocus(false);
    }
}

$(document).keyup(function (e) {
    var input = $("input");
    if (!input.is(":visible")) return;

    if (e.which === 38) {
        if (messages.length > 0) {
            if (messageidx == (messages.length - 1))
                return;

            messageidx++;
            $("input").val(messages[messageidx]);
        }
    }
    else if (e.which === 40) {
        if (messageidx < 0) {
            $("input").val(null);
            return;
        }

        messageidx--;
        $("input").val(messages[messageidx]);        
    }
});