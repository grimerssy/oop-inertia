<script>
    import {onMount} from "svelte";

    import {request} from './store.js'
    import {bestPlayer} from "./store.js"

    import empty from '../../static/empty.png'
    import sandpit from "../../static/sandpit.png"
    import rock from "../../static/rock.png"
    import ant from "../../static/ant.png"
    import cactus from "../../static/cactus.png"
    import activePlayer from "../../static/armadillo-curled.png"
    import idlePlayer from "./../../static/armadillo-standing.png"
    import deadPlayer from './../../static/armadillo-sitting.png'

    const idleState = 0
    const activeState = 1
    const deadState = 2

    let cells = [[]]
    let players = []
    let pointsObjective

    let fieldWidth
    let fieldHeight

    const maxAnimationMs = 250
    const accelerationPercentage = 25
    const minAnimationMs = 50

    onMount(async () => {
        const response = await fetch(`http://localhost:8000/api/inertia`, {
            method: 'POST',
            body: JSON.stringify(request),
            headers: {
                'Content-Type': 'application/json'
            },
        })

        const result = await response.json()

        cells = result.cellTypes
        players = result.players
        pointsObjective = result.pointsObjective

        fieldWidth = cells.length
        fieldHeight = cells[0].length

        await displayPlayers()
        await listen()
    })

    async function displayPlayers() {
        await new Promise(r => setTimeout(r, 1));

        players.forEach(p => {
            const id = p.coordinate.x + ":" + p.coordinate.y
            const cell = document.getElementById(id)
            const img = cell.firstElementChild.firstElementChild

            switch (p.state) {
                case idleState:
                    img.src = idlePlayer
                    break
                case activeState:
                    img.src = activePlayer
                    break
                case deadState:
                    img.src = deadPlayer
                    break
            }

            cell.style.background = p.color + "40"
        })
    }

    let index = 0

    function getNextPlayer() {
        let found = false
        while(!found) {
            index++

            index = index < players.length ? index : 0

            if (players[index].state === 2) {
                continue
            }

            return players[index]
        }
    }

    let listening = true

    function getDirectionFromChar(char) {
        switch (char) {
            case 'Q':
                return "tl"
            case 'W':
                return "t"
            case 'E':
                return "tr"
            case 'A':
                return "l"
            case 'S':
                return "b"
            case 'D':
                return "r"
            case 'Z':
                return "bl"
            case 'C':
                return "br"
        }
    }

    function getImage(imageName) {
        switch (imageName) {
            case 'empty':
                return empty
            case 'trap':
                return cactus
            case 'prize':
                return ant
            case 'wall':
                return rock
            case 'stop':
                return sandpit
        }
    }

    function getChar(event) {
        let charCode = (typeof event !== 'undefined') ? event.keyCode : event.which

        return String.fromCharCode(charCode).toUpperCase();
    }

    function loadToBestPlayer(_bestPlayer) {
        bestPlayer.name = _bestPlayer.name
        bestPlayer.score = _bestPlayer.score
        bestPlayer.coordinate = _bestPlayer.coordinate
        bestPlayer.color = _bestPlayer.color
    }

    function displayEndScreen() {
        document.getElementById('root').innerHTML = `<div style=" display: flex;flex-direction: column;justify-content: center;">
                                                                <h2 style="margin: 5rem auto;">Game over!</h2>
                                                                <a style="background: var(--main-white);padding:0.7rem;border-radius: 0.7rem; margin: 1rem auto;" sveltekit:prefetch href="/game/results">see results</a>
                                                             </div>`
    }

    function updatePlayerDiv(div, player) {
        div.style.color = player.color+ "C0"
        div.innerText = player.name
    }

    async function listen() {
        const activeButtons = ['Q', 'W', 'E', 'A', 'S', 'D', 'Z', 'C']

        let currentPlayer = players[index]

        const currentPlayerDiv = document.getElementById('current-player')
        updatePlayerDiv(currentPlayerDiv, currentPlayer)

        document.onkeydown = async event => {
            if (!listening) {
                return
            }

            let char = getChar(event)

            if (activeButtons.indexOf(char) === -1) {
                return
            }

            listening = false

            let id = currentPlayer.coordinate.x + ":" + currentPlayer.coordinate.y
            let cell = document.getElementById(id)
            let img = cell.firstElementChild.firstElementChild
            img.src = activePlayer

            let direction = getDirectionFromChar(char)

            const color = currentPlayer.color.substring(1)
            let moving = true

            let animationMs = maxAnimationMs

            while(moving) {
                const response = await fetch(`http://localhost:8000/api/inertia/${color}/${direction}`, {
                    method: 'PUT',
                })
                const result = await response.json()

                players[index] = result.player
                currentPlayer = result.player

                id = result.prevCoordinate
                cell = document.getElementById(id)
                img = cell.firstElementChild.firstElementChild

                cell.style.background = 'none'

                img.src = getImage(result.prevCellType)

                id = result.player.coordinate.x + ":" + result.player.coordinate.y
                cell = document.getElementById(id)
                img = cell.firstElementChild.firstElementChild

                cell.style.background = currentPlayer.color + "40"

                await displayPlayers()


                if (currentPlayer.state === activeState) {
                    await new Promise(r => setTimeout(r, animationMs));
                    continue
                }

                moving = false

                await new Promise(r => setTimeout(r, animationMs));

                const newDelay = animationMs - animationMs * accelerationPercentage / 100
                animationMs = Math.max(newDelay, minAnimationMs)
            }

            if (players.filter(p => p.state !== deadState).length === 0 ||
                players.filter(p => p.score >= pointsObjective).length !== 0) {

                await fetch(`http://localhost:8000/api/inertia/results/save`, {
                    method: 'POST',
                })

                await new Promise(r => setTimeout(r, 1700))

                const bestScore = players.reduce((prev, next) => Math.max(prev, next.score), 0)
                const _bestPlayer = players.find(p => p.score === bestScore)

                loadToBestPlayer(_bestPlayer)

                displayEndScreen()
                return
            }

            currentPlayer = getNextPlayer()

            updatePlayerDiv(currentPlayerDiv, currentPlayer)
            listening = true
        }
    }
</script>

<div id="root">
    <div class="row m-y">
        <div class="field-wrapper w-60">
            <div class="field">
                <div class="row">
                    {#each cells as column, x}
                        <div class="col field-col">
                            {#each column as cell, y}
                                <div id={x + ":" + y} class="cell">
                                    {#if cell === 'empty'}
                                        <div class="empty">
                                            <img src={empty} alt=" ">
                                        </div>
                                    {/if}
                                    {#if cell === 'prize'}
                                        <div class="prize">
                                            <img src={ant} alt="ant">
                                        </div>
                                    {/if}
                                    {#if cell === 'trap'}
                                        <div class="trap">
                                            <img src={cactus} alt="cactus">
                                        </div>
                                    {/if}
                                    {#if cell === 'stop'}
                                        <div class="stop">
                                            <img src={sandpit} alt="sandpit">
                                        </div>
                                    {/if}
                                    {#if cell === 'wall'}
                                        <div class="wall">
                                            <img src={rock} alt="rock">
                                        </div>
                                    {/if}
                                </div>
                            {/each}
                        </div>
                    {/each}
                </div>
            </div>
        </div>
        <div class="w-40 col">
            <div class="row m-y">
                Controls
            </div>
            <div class="row m-y">
                <div class="col m-x">
                    <div>↖</div>
                    <p>[Q]</p>
                </div>
                <div class="col m-x">
                    <div>↑</div>
                    <p>[W]</p>
                </div>
                <div class="col m-x">
                    <div>↗</div>
                    <p>[E]</p>
                </div>
            </div>
            <div class="row m-y">
                <div class="col m-x">
                    <div>←</div>
                    <p>[A]</p>
                </div>
                <div class="col m-x">
                    <div>→</div>
                    <p>[D]</p>
                </div>
            </div>
            <div class="row m-y">
                <div class="col m-x">
                    <div>↙</div>
                    <p>[Z]</p>
                </div>
                <div class="col m-x">
                    <div>↓</div>
                    <p>[S]</p>
                </div>
                <div class="col m-x">
                    <div>↘</div>
                    <p>[C]</p>
                </div>
            </div>
            <div class="m-y">
                objective: {pointsObjective} points
            </div>
        </div>
    </div>
    <div class="row m-y">
        <div class="row even w-60">
            {#each players as player}
                <div class="col player-box">
                    <p style="color: {player.color}">
                        {player.name}<br>
                        Health: {player.health}<br>
                        Score: {Math.floor(player.score)}
                    </p>
                </div>
            {/each}
        </div>
        <div class="row w-40 m-y">
            <div id="current-player"></div>
            <div>'s turn</div>
        </div>
    </div>
</div>

<style>
    img {
        width: 100%;
        height: 100%;
    }

    p {
        font-size: 0.7rem;
    }

    .m-y {
        margin: 0.5rem 0;
    }

    .m-x {
        margin: 0 1rem;
    }

    .player-box {
        padding: 0 0.7rem;
        border-radius: 0.7rem;
        background: var(--main-white);
    }

    .w-40 {
        width: 40%;
    }

    .row {
        display: flex;
        text-align: center;
        justify-content: center;
    }

    .even {
        justify-content: space-evenly !important;
    }

    .col {
        display: flex;
        flex-direction: column;
    }

    .w-60 {
        width: 60%;
    }

    .trap,
    .empty,
    .prize,
    .wall,
    .stop {
        width: 100%;
        height: 100%;
    }

    .cell {
        margin: 0.1rem;
        border-radius: 0.6rem;
        padding: 0.1rem;
    }

    .field-wrapper {
        padding: 0.4rem;
        border-radius: 0.7rem;
        background: var(--main-white);
        height: 100%;
    }

    .field {
        background: linear-gradient(
                180deg,
                #FFFFBE 0%,
                #FFFFBF 100%
        );
        padding: 0.1rem;
        border-radius: 0.5rem;
    }

    .field-col {
        margin-top: 0.2rem;
    }
</style>