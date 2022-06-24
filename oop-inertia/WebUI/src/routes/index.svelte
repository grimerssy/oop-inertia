<script>
    import preview from '../static/placeholder.svg'
</script>

<div>
    <div>
        <img src={preview} alt="preview">
    </div>
    <div>
        <a sveltekit:prefetch href="/game">Play</a>
    </div>
</div>

<style>
    div {
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: space-evenly;
    }

    div div {
        width: 50%;
    }

    img {
        max-width: 250px;
        width: 80%;
    }

    a {
        font-size: 2rem;
        padding: 1rem;
        background: var(--main-white);
        border-radius: 0.5rem;
    }
</style>