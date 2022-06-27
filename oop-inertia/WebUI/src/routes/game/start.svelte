<script>
    let inputField
    let fieldWidthSlider
    let fieldHeightSlider
    let names = []
    import {request} from './store.js'
</script>

<div class="row">
    <div class="w-50">
        <h2>Field</h2>
    </div>
    <div class="w-50">
        <h2>Players</h2>
    </div>
</div>
<div class="row">
    <div class="w-50 col">
        <h3 class="m-top">width</h3>
        <div class="slider">
            <input bind:this={fieldWidthSlider} type="range" min="10" max="30" value="20" oninput="this.nextElementSibling.value = this.value">
            <output>20</output>
        </div>
        <h3 class="m-top">height</h3>
        <div class="slider">
            <input bind:this={fieldHeightSlider} type="range" min="5" max="15" value="10" oninput="this.nextElementSibling.value = this.value">
            <output>10</output>
        </div>
    </div>
    <div class="w-50 col m-top">
        <div class="inputField">
            <input bind:this={inputField} type="text" placeholder="name">
            <button type="button" on:click={() => {names = [...names, inputField.value === '' ? 'guest' : inputField.value]; inputField.value = ''}}>+</button>
            <button type="button" on:click={() => names = []}>↺</button>
        </div>
        {#each names as name}
            <div class="inputField">
                <input type="text" value={name} readonly>
            </div>
        {/each}
    </div>
</div>
<div class="row">
    <a sveltekit:prefetch href="/game">
        <input class="m-top" type="submit" value="Start" on:click={() => {
            request.fieldWidth = fieldWidthSlider.value
            request.fieldHeight = fieldHeightSlider.value
            if (names.length === 0) {
                names = ["guest"]
                alert("Cannot have a game without players.\n" +
                "Started a one player game with name \"guest\".")
            }
            request.playerNames = names.length === 0 ? ["guest"] : names
        }}>
    </a>
</div>

<style>
    input {
        align-self: center;
    }

    input:focus {
        outline: none;
    }

    input[type="submit"]:hover {
        color: var(--accent-color);
    }

    input[type="text"], button {
        background: var(--main-white);
        border: 0;
        border-radius: 0.3rem;
    }

    input[type="text"]{
        padding: 0.3rem 0 0.3rem 0.5rem;
        font-size: 0.7rem;
        align-self: center;
        height: 80%;
    }

    button {
        height: 100%;
        text-align: center;
        width: 1.5rem;
        margin: 0 0.3rem;
    }

    button:active {
        background: var(--secondary-white);
    }

    button:hover {
        color: var(--accent-color);
    }

    input[type="range"] {
        -webkit-appearance: none;
        width: 100%;
        height: 0.5rem;
        border-radius: 5px;
        background: var(--main-white);
        outline: none;
        -webkit-transition: .2s;
        transition: opacity .2s;
    }

    input[type="range"]::-webkit-slider-thumb {
        -webkit-appearance: none;
        appearance: none;
        width: 23px;
        height: 24px;
        border: 0;
        border-radius: 50%;
        background-image: url("../../static/armadillo-standing.svg");
        background-size: contain;
        background-position: center center;
        background-repeat: no-repeat;
        cursor: pointer;
    }

    input[type="range"]::-moz-range-thumb {
        width: 23px;
        height: 24px;
        border: 0;
        border-radius: 50%;
        background-image: url('https://img.icons8.com/material-outlined/344/average-2.png');
        background-size: contain;
        background-position: center center;
        background-repeat: no-repeat;
        cursor: pointer;
    }

    input[type="range"]::-webkit-slider-thumb:active {
        background-image: url("../../static/armadillo-curled.svg");
    }

    output {
        font-size:0.7rem;
        width: 10%;
        background: var(--main-white);
        padding: 0.3rem;
        border-radius: 1rem;
        text-align: center;
        color: var(--text-color)
    }

    h3 {
        margin-bottom: 0;
    }

    .inputField {
        display: flex;
        text-align: center;
        justify-content: center;
        margin: 0.5rem 0 0.5rem 0;
    }

    .row {
        display: flex;
        text-align: center;
        justify-content: center;
    }

    .col {
        display: flex;
        flex-direction: column;
    }

    .slider {
        width: 50%;
        display: flex;
        align-items: center;
        align-self: center;
    }

    .m-top {
        margin-top: 2rem;
    }

    .w-50 {
        width: 50%;
    }
</style>