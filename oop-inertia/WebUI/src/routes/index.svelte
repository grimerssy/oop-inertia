<script>
    import preview from '../static/placeholder.svg'
</script>

<div>
    <div class="w-50">
        <img id="preview" src={preview} alt="preview">
    </div>
    <div class="w-50">
        <a id="play" sveltekit:prefetch href="/game/start">Play</a>
    </div>
</div>

<style>
    div {
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: space-evenly;
    }

    .w-50 {
        width: 50%;
    }

    #preview {
        max-width: 250px;
        width: 80%;
    }

    #play {
        font-size: 2rem;
        padding: 1rem;
        background: var(--main-white);
        border-radius: 0.5rem;
    }
</style>