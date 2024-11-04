import { Icon, IconProps } from './Icon'

function IconChevronDown(props: IconProps) {
    return (
        <Icon {...props}>
            <path
                fillRule="evenodd"
                d="M1.646 4.646a.5.5 0 0 1 .708 0L8 10.293l5.646-5.647a.5.5 0 0 1 .708.708l-6 6a.5.5 0 0 1-.708 0l-6-6a.5.5 0 0 1 0-.708"
            />
        </Icon>
    )
}

export { IconChevronDown }
