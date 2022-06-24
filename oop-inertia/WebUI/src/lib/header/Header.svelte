<script>
    import letterE from './armadillo-letter-e.svg';
</script>

<header>
    <nav>
        <svg viewBox="0 0 2 3" aria-hidden="true">
            <path d="M0,0 L1,2 C1.5,3 1.5,3 2,3 L2,0 Z" />
        </svg>
        <h1>
            <a sveltekit:prefetch href="/">
                In<img src={letterE} alt="e">rtia
            </a>
        </h1>
        <svg viewBox="0 0 2 3" aria-hidden="true">
            <path d="M0,0 L0,3 C0.5,3 0.5,3 1,2 L2,0 Z" />
        </svg>
    </nav>
</header>

<style>
    header {
        display: flex;
        justify-content: center;
    }

    h1 {
        background: var(--main-white);
        margin: 0;
        padding-top: 5px
    }

    img {
        height: 1.5rem;
    }

    nav {
        display: flex;
        justify-content: center;
        --background: var(--main-white);
    }

    svg {
        width: 2em;
        height: 3em;
        display: block;
    }

    path {
        fill: var(--background);
    }
</style>
