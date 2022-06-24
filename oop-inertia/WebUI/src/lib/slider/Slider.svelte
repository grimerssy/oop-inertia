<div>
    <input type="range" min="5" max="50" value="30" oninput="this.nextElementSibling.value = this.value">
    <output>30</output>
</div>

<style>
    div {
        width: 50%;
        display: flex;
        align-items: center;
        justify-content: space-evenly;
    }

    input {
        -webkit-appearance: none;
        width: 100%;
        height: 0.5rem;
        border-radius: 5px;
        background: var(--main-white);
        outline: none;
        -webkit-transition: .2s;
        transition: opacity .2s;
    }

    input::-webkit-slider-thumb {
        -webkit-appearance: none;
        appearance: none;
        width: 23px;
        height: 24px;
        border: 0;
        border-radius: 50%;
        background-image: url("thumb-unclicked.svg");
        background-size: contain;
        background-position: center center;
        background-repeat: no-repeat;
        cursor: pointer;
    }

    input::-moz-range-thumb {
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

    input::-webkit-slider-thumb:active {
        background-image: url("thumb-clicked.svg");
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
</style>