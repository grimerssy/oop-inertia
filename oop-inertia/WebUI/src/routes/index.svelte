<script>
    import preview from '../static/demo.webp'
</script>

<div class="m-y">
    <div class="w-50">
        <img id="preview" src={preview} alt="preview">
    </div>
    <div class="col w-50">
        <a id="play" sveltekit:prefetch href="/game/start">Play</a>
        <a id="leaderboard" sveltekit:prefetch href="/leaderboard">Leaderboard</a>
    </div>
</div>

<style>
    div {
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: space-evenly;
    }

    img {
        padding: 0.5rem;
        border-radius: 0.7rem;
        background: var(--main-white);
        width: 100%;
        margin: 0;
    }

    .col {
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
    }

    .m-y {
        margin: 2.5rem 0;
    }

    .w-50 {
        width: 50%;
    }

    #preview {
        max-width: 250px;
        width: 80%;
    }

    #play, #leaderboard {
        margin: 2rem auto;
        font-size: 2rem;
        padding: 1rem;
        background: var(--main-white);
        border-radius: 0.5rem;
    }
</style>