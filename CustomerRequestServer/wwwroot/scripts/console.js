let connection;
function toggleConsole() {
    const consoleContainer = document.querySelector('.console-container');
    const toggleIcon = document.querySelector('.toggle-icon');

    if (consoleContainer.classList.contains('open')) {
        consoleContainer.classList.remove('open');
        toggleIcon.innerHTML = '&#9660;';
    } else {
        consoleContainer.classList.add('open');
        toggleIcon.innerHTML = '&#9650;';
        scrollConsoleToBottom();
    }
}

function streamConsoleLog(data, isUserInput = false) {
    const consoleOutput = document.getElementById('consoleOutput');
    let converter = new showdown.Converter();

    let formattedData = data;
    if (isUserInput) {
        formattedData = `<strong>></strong> ${data}`;
    }

    const markdownOutput = converter.makeHtml(formattedData);
    consoleOutput.innerHTML += `<div class="console-message${isUserInput ? ' user-input' : ''}">${markdownOutput}</div>`;

    // Apply syntax highlighting to the code blocks
    const codeBlocks = consoleOutput.querySelectorAll('code');
    codeBlocks.forEach((block) => {
        hljs.highlightBlock(block);
    });

    scrollConsoleToBottom();
}

function scrollConsoleToBottom() {
    const consoleContent = document.querySelector('.console-content');
    consoleContent.scrollTop = consoleContent.scrollHeight;
}

function sendConsoleCommand() {
    const consoleInput = document.getElementById('consoleInput');
    const command = consoleInput.value;
    if (command.trim() !== '') {
        streamConsoleLog(command, true);
        consoleInput.value = '';
        consoleInput.focus();

        // Send the command to the server
        connection.invoke("ExecuteConsoleCommand", command).catch(function (err) {
            streamConsoleLog(`Error: ${err.toString()}`);
        });
    }
}

async function onConsoleKeyUp(event) {
    if (event.key === 'Enter') {
        await sendConsoleCommand();
    }
}

function initializeConsole() {
    document.getElementById('consoleInput').addEventListener('keyup', onConsoleKeyUp);

    connection = new signalR.HubConnectionBuilder()
        .withUrl("/devConsoleHub")
        .build();

    connection.on("PushConsoleMessage", function (message) {
        streamConsoleLog(message);
    });

    connection.start().catch(function (err) {
        console.error(err.toString());
    });
}

document.addEventListener('DOMContentLoaded', initializeConsole);