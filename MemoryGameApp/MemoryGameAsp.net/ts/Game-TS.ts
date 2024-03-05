namespace gameTs {
    const rbSolo = document.getElementById('solo');
    const currentTurnMsg = ["Player 1's Turn", "Player 2's Turn", "Computer's Turn", "Player 1 Won!", "Player 2 Won!", "Tie", "Computer Won!"];
    const prevPickedCards = [];



    let allCards;
    let startBtn;
    let msg;
    let playerOptions;
    let gameStatus = 0;
    let isGameSolo = false;
    let currentTurn = 0;
    let player1Score = 0;
    let player2Score = 0;
    let comPick1;
    let comPick2;


    $(document).ready(function () {
        //Variable assignments
        startBtn = $("#btnStart");
        playerOptions = $(".my-radio");
        msg = $("#gameMsg");
        allCards = $('.mycard');

        //Event Handlers
        playerOptions.change(gameTypeMsgs);
        startBtn.click(startGame);
        allCards.click(doMove);
    });
    //Functions
    const setTurnMsg = () => msg.text(currentTurnMsg[currentTurn]);
    const getRndNum = maxExcluded => Math.floor((Math.random() * maxExcluded));
    const showScore = () => { $('#player1Sets').text(player1Score); $('#player2Sets').text(player2Score); }
    const gameTypeMsgs = event => { $("#gameType").text(event.target.value); $("#player2name").text(rbSolo.checked ? "Computer Sets:" : "Player 2 Sets:"); }



    function startGame() {
        if (!gameStatus) {
            shuffleCards();
            isGameSolo = rbSolo.checked;
            currentTurn = 0;
            prevPickedCards.length = 0;
        }
        allCards.removeClass('picked claimed');
        player1Score = 0;
        player2Score = 0;
        showScore();
        setGameMsgs();
        gameStatus == 1 ? gameStatus = 0 : gameStatus = 1;
    }

    function setGameMsgs() {
        if (gameStatus) {
            startBtn.text("Start Game");
            msg.text("Press Start to Start Game");
            playerOptions.removeAttr('disabled');
            msg.removeClass('text-warning');
        }
        else {
            startBtn.text("Restart Game");
            playerOptions.attr('disabled', true);
            msg.addClass('text-warning');
            msg.removeClass('bg-success');
            setTurnMsg();
        }
    }

    function doMove(event) {
        let card = event.target;
        if (!gameStatus || card.classList.contains('picked') || $('.picked').length === 2 || (currentTurn === 2 && event.originalEvent.isTrusted)) { return; }

        $(card).addClass('picked');
        if (isGameSolo && !prevPickedCards.includes(card)) { prevPickedCards.push(card); }

        if ($('.picked').length === 2) {
            setTimeout(() => checkMatch(), 2000);
        }
    }

    function checkMatch() {
        let pickedCards = $('.picked');
        if (pickedCards.eq(0).text() === pickedCards.eq(1).text()) {
            currentTurn === 0 ? player1Score++ : player2Score++;
            showScore();
            pickedCards.addClass('claimed');
            pickedCards.each(() => prevPickedCards.splice(prevPickedCards.indexOf(this)), 1);
        }

        pickedCards.removeClass('picked');
        setGameStatusAndMsg();
    }

    function setGameStatusAndMsg() {
        if ($('.claimed').length === 20) {
            gameStatus = 0;
            if (player1Score === player2Score) { currentTurn = 5; }
            else { currentTurn = player1Score > player2Score ? 3 : isGameSolo ? 6 : 4; }
            msg.addClass('bg-success');
            playerOptions.removeAttr('disabled');
        };
        if (gameStatus) { currentTurn = currentTurn === 1 || currentTurn === 2 ? 0 : isGameSolo ? 2 : 1; };
        setTurnMsg();
        if (currentTurn === 2) { doComputerMove(); }
    }


    function doComputerMove() {
        if (gameStatus) {
            if (!pickedCardsMatch()) {
                let unclaimedCards = [...document.querySelectorAll(".mycard:not(.claimed)")];
                comPick1 = unclaimedCards[getRndNum(unclaimedCards.length)];
                comPick2 = null;

                // Check if comPick1 is a match to any previously picked cards
                prevPickedCards.forEach(c => {
                    if (c.innerHTML === comPick1.innerHTML) { comPick2 = c; }
                })

                // If not assaign to random card
                comPick2 = comPick2 || unclaimedCards[getRndNum(unclaimedCards.length)];

                // While loop to make sure not picking the same card twice
                while (comPick2 === comPick1) { comPick2 = unclaimedCards[getRndNum(unclaimedCards.length)]; }
            }

            // Pick cards with delay
            setTimeout(() => {
                comPick1.click();
                setTimeout(() => comPick2.click(), 1800);
            }, 2000);
        }

    }

    function pickedCardsMatch() {
        let pickedCount = prevPickedCards.length;
        if (pickedCount > 1) {
            for (let i = 0; i < pickedCount; i++) {
                for (let j = i + 1; j < pickedCount; j++) {
                    if (prevPickedCards[i].innerHTML === prevPickedCards[j].innerHTML) {
                        comPick1 = prevPickedCards[i];
                        comPick2 = prevPickedCards[j];
                        return true;
                    }
                }
            }
        }
        return false;
    }

    function shuffleCards() {
        let cards = [];
        cards = [...document.querySelectorAll('.mycard')];
        let cardPairs = [];
        while (cards.length > 1) {
            let i = getRndNum(cards.length);
            let j = getRndNum(cards.length);
            while (i === j) {
                j = getRndNum(cards.length);
            }

            cardPairs.push([cards[i], cards[j]]);

            cards.splice(i, 1);
            if (i < j) { j-- };
            cards.splice(j, 1);
        }

        for (let k = 0; k < cardPairs.length; k++) {
            cardPairs[k][0].innerHTML = String.fromCharCode(73 + k);
            cardPairs[k][1].innerHTML = String.fromCharCode(73 + k);
        }
    }

}